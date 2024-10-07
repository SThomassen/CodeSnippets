import Vue from 'vue'
import firebase from '@/plugins/firebase'

const shareDB = firebase.firestore().collection('shared_projects');

export const state = () => ({
    user: null,
    users: {},
    builders: {},
    projects: {},
});

export const getters = {
    users(state) {
        return state.users;
    },
    projects(state) {
        return state.projects;
    }
}

export const mutations = {
    setShared(state, payload) {
        Vue.set(state.projects, payload.id, payload.value);
    },
    setUser(state, payload) {
        Vue.set(state.user, payload.id, payload);
    },
    setBuilder(state, payload) {
        Vue.set(state.builders, payload.id, payload);
    },
    setProject(state, payload) {
        if (state.projects[payload.projectId] === undefined) state.projects[payload.projectId] = {};
        Vue.set(state.projects[payload.projectId], payload.userId, payload.value );
    }
}

export const actions = {
    login(context, payload) {
        return new Promise( (resolve, reject) => {
            try {
                console.log('auth login', payload.uid);
                // register for live update when user data changes
                context.state.register[payload.uid] = userDB.doc(payload.uid).onSnapshot( snapshot => {
                    if (!snapshot.exists || snapshot.data().username === undefined) return;
                    
                    let user = {
                        id: snapshot.id, 
                        ...snapshot.data(),
                        providerData: payload.providerData,
                        emailVerified: payload.emailVerified, 
                    }
                    context.commit('setUser', user);
                    context.commit('setBuilder', user);
                })
                resolve();
            } catch (err) {
                reject();
            }
        })
    },
    getSharedByProjectId(context, payload) {
        return new Promise( async (resolve, reject) => {
            try {
                console.log('shared getSharedByProjectId', payload);
                let doc = await shareDB.doc(payload).get();
                if (!doc.exists) throw 'Document does not exist';

                context.commit('setShared', { id: payload, value: doc.data() });
                resolve();
            } catch(err) {
                reject(err);
            }
        })
    },
    getSharedByUserId(context, payload) {
        return new Promise( async (resolve, reject) => {
            try {
                let user = (payload) ? payload : firebase.auth().currentUser.uid;
                let snapshot = await shareDB.where(user,'in',['VWjXPWccqBCRu9SXQjQd', 'j46wV85HuRVGNDoC5NiK']).get();
                snapshot.forEach( doc => {
                    context.commit('setShared', { id: doc.id, value: doc.data() });
                })
                resolve();
            } catch(err) {
                reject(err);
            }
        })
    },
    setSharedByProjectId(context, payload) {
        return new Promise( async (resolve, reject) => {
            try {
                console.log('set shared projects', payload.id);
                await shareDB.doc(payload.id).set(payload.value);
                context.commit('setShared', payload)
                resolve();
            } catch(err) {
                reject(err);
            }
        })
    }
}