// Update user data
exports.updateUser = functions.https.onCall( async (data, context) => {
    try {
        console.log("user",data);
        await admin.firestore().collection('users').doc(data.id).update(data);
        await admin.auth().updateUser(data.id, {
            displayName: data.displayName
        })

        return { message: "Request has successfully been verified." };
    } catch (err) {
        throw err;
    }
})

// Send welcome email when a new user is created
exports.welcomeEmail = functions.auth.user().onCreate( async user => {
    const actionCodeSettings = {
        url: "https://www.layhgobuilder.com",
        handleCodeInApp: false
    }
    let link = await admin.auth().generateEmailVerificationLink(user.email, actionCodeSettings)
    const msg = {
        to: user.email,
        from: 'no-reply@layhgobuilder.com',
        templateId: 'd-394189a0cb8c4449b8c0c4baccc86f2b',
        dynamic_template_data: {
            name: user.displayName,
            verifyURL: link,
        },
    };

    // Email is send with the help of SendGrid
    await sgMail.send(msg);

    // Also send an email to the admin to let them know a new user has signed up
    const msgAdmin = {
        to: 'layhgobuilder@gmail.com',
        from: 'no-reply@layhgobuilder.com',
        templateId: 'd-192c7b53b2ae46fe9ca8e8f52efbe8b8',
        dynamic_template_data: {
            name: user.displayName,
            email: user.email,
            uid: user.uid
        },
    };

    return sgMail.send(msgAdmin);
});

// Send email with link to change their password to the user
exports.forgotPasswordEmail = functions.https.onCall( async (data, context) => {
    try {
        const actionCodeSettings = {
            url: "https://www.layhgobuilder.com",
            handleCodeInApp: false
        }

        // Check if email is known at the database
        let user = await admin.auth().getUserByEmail(data.email);
        if (!user) {
            throw `No user with the email ${data.email}.`
        }

        // Firebase generates a reset password link
        let link = await admin.auth().generatePasswordResetLink(data.email, actionCodeSettings);
        console.log('generated link: ', link);
        const msg = {
            to: data.email,
            from: 'no-reply@layhgobuilder.com',
            templateId: 'd-de6d05f1fc6443349696e94894da8f57',
            dynamic_template_data: {
                name: user.displayName,
                resetURL: link,
            }
        }
        console.log("send msg to: ", data.email);
        return sgMail.send(msg);
    } catch(err) {
        return err;
    }
})

// HTTP Unity Requests

// Authentication happens when Unity WebGL loading is successful.
exports.authenticateUser = functions.https.onRequest( (req, res) => {
    cors(req, res, async () => {
        if (req.method !== 'POST') {
            return res.status(500).json({ message: 'Request type is not allowed' });
        }

        try {
            console.log(req.body.token);

            let decoded = await admin.auth().verifyIdToken(req.body.token);
            
            console.log('decoded ', JSON.stringify(decoded));
            return res.status(200).json(decoded);
        }
        catch (error) {
            return res.status(500).send( error );
        }
    });
})

// Request from Unity WebGL to open a project
exports.loadProject = functions.https.onRequest( (req, res) => {
    cors(req, res, async() => {
        if (req.method !== 'POST') {
            return res.status(500).json({ message: 'Request type is not allowed' });
        }

        //todo: check security rules
        try {
            // Get project from the firestore
            let projectRef = admin.firestore().collection('projects').doc(req.body.id);
            let projectDoc = await projectRef.get()
            if (!projectDoc.exists)
                throw 'no project found';
            
            // Add project id to the json data
            let project = { id: projectDoc.id, ...projectDoc.data() };
            // Get the latest save data from the project
            let saveDoc = await projectRef.collection('saves').doc(project.lastSave).get();

            // Return the project data to Unity WebGL
            return res.status(200).json({ 
                ...project, 
                saves: { 
                    ...saveDoc.data()
                }
            });
        }
        catch(error) {
            return res.status(500).send(error);
        }
    })
})

// Request to save the Unity WebGL project to the firestore
exports.saveProject = functions.https.onRequest( (req, res) => {
    cors(req, res, async() => {
        if (req.method !== 'POST') {
            return res.status(500).json({ message: 'Request type is not allowed' });
        }
        try {
            // Get the server data
            let timestamp = admin.firestore.FieldValue.serverTimestamp();
            let increment = admin.firestore.FieldValue.increment(1);

            // parse request data to valid project data
            let projectJson = JSON.parse(req.body.project);
            let savesJson = Object.assign(projectJson.saves);
            let project = {
                "--stats--": {
                    votes: 0,
                    views: 0,
                    visits: 0,
                    comments: 0
                },
                invoked: false,
                showcase: false,
                published: false,
                allowComments: false,
                type: projectJson.type,
                version: projectJson.version,
                creatorId: projectJson.creatorId,
                creatorName: projectJson.creatorName,
                projectName: projectJson.projectName,
                creationTime: timestamp,
                lastUpdate: timestamp,
                state: 'pending'
            }
            let objects = [];
            savesJson.created = admin.firestore.FieldValue.serverTimestamp();
            
            let usersRef = admin.firestore().collection('users');
            let projectsRef = admin.firestore().collection('projects');
            let projectRef = projectsRef.doc();

            // Create new or overwrite project at the Firestore document
            await projectRef.set( { id: projectRef.id, ...project });
        
            // Update statistics and user data
            let batch = admin.firestore().batch();
            let statsRef = projectsRef.doc('--stats--');
            let saveRef = projectRef.collection('saves').doc();
            let userRef = usersRef.doc(projectJson.creatorId);

            batch.set(saveRef, { objects: objects });
            batch.set(statsRef, { total: increment }, { merge: true });
            batch.set(projectRef, { lastSave: saveRef.id }, { merge: true } );
            batch.update(userRef, { '--stats--.projects': increment });

            await batch.commit();

            return res.status(200).json({ ...project, id: projectRef.id });
        }
        catch(error) {
            return res.status(500).send(error);
        }
    })
})