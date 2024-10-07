<template>
    <v-container>
        <v-form ref='loginForm' @submit.prevent='onSignInEmail'>
            <v-row justify='center' align='center' no-gutters>
                <v-col cols='10'>
                    <v-row no-gutters>
                        <v-col cols='12'>
                            <v-text-field
                                dense
                                outlined
                                :label='$t("email")'
                                :rules='rules.email'
                                v-model='form.email'>
                                <template v-slot:message='item'>
                                    {{ $t(item.message) }}
                                </template>
                            </v-text-field>        
                        </v-col>

                            <v-col cols='12'>
                            <v-text-field 
                                dense 
                                outlined 
                                type='password'
                                :label='$t("password")'
                                :rules='rules.password'
                                v-model='form.password'>
                                <template v-slot:message='item'>
                                    {{ $t(item.message) }}
                                </template>
                            </v-text-field>
                        </v-col>

                        <v-col cols='12'>
                            <span class='caption red--text'>{{error}}</span>
                        </v-col>
                    </v-row>

                    <v-row no-gutters class='mb-5'>
                        <v-col cols='12'>
                            <v-btn block outlined type='submit' :loading="running">
                                <v-icon left>mdi-login</v-icon>
                                {{ $t("log_in") }}
                            </v-btn>
                        </v-col>
                        <v-col cols='12'>
                            <v-btn x-small block text @click='forgotPassword'>
                                {{ $t("forgot_password") }}
                            </v-btn>
                        </v-col>
                    </v-row>

                </v-col>
            </v-row>
        </v-form>

        <v-card flat color='provider'>
            <!-- <v-row no-gutters justify="center">
                <v-col cols='12' class="text-center">
                    <span class='font-weight-light'>{{ $t('or') }}</span>
                </v-col>
            </v-row> -->
            <!-- <v-row justify="center">
                <v-col cols='8'>
                    <v-btn block rounded depressed color='#3b5998' dark @click="onSignInFacebook">
                        <v-icon left>mdi-facebook</v-icon>
                        <span>Facebook</span>
                    </v-btn>
                </v-col>
                <v-col cols='8'>
                    <v-btn block rounded depressed @click="onSignInGoogle">
                        <v-icon left>mdi-google</v-icon>
                        <span>Google</span>
                    </v-btn>
                </v-col>
            </v-row> -->
            

            <v-row no-gutters class='mt-6'>
                <v-col cols='12'>
                    <v-btn block x-small outlined @click='$emit("signup")'>
                        {{ $t('signup_account')}}
                        <v-icon right>mdi-arrow-right</v-icon>
                    </v-btn>
                </v-col>
            </v-row>
        </v-card>
    </v-container>
</template>

<script>
import firebase from '@/plugins/firebase'

export default {
    data() {
        return {
            running: false,
            error: "",
            form: {},
            rules: {
                email: [
                    v => !!v || 'rules.email_required',
                    v => /.+@.+/.test(v) || 'rules.email_valid'
                ],
                password: [
                    v => (v && v.length >= 6) || 'rules.password_length'
                ]
            }
        }
    },
    methods: {
        cancel() {
            this.error = "";
            this.$refs.loginForm.reset();

            switch (this.$route.query.show) {
                case "fieldlab_license":
                    this.$emit('fieldlab');
                    break;
                case "enterprise_license":
                    this.$emit('enterprise');
                    break;
                default:
                    this.$emit('cancel');
                    break;
            }
        },
        open() {
            this.error = "";
        },
        forgotPassword() {
            this.$emit('forgot');
        },
        async onSignInEmail() {
            if (!this.$refs.loginForm.validate()) {
                return;
            }
            
            this.running = true;
            try {
                await firebase.auth().signInWithEmailAndPassword(this.form.email, this.form.password);
                this.running = false;
                this.cancel();
            } catch(error) {
                this.error = error.message;
                this.running = false;
            }
        },
        async onSignInFacebook() {
            this.running = true;
            try {
                var provider = new firebase.auth.FacebookAuthProvider();

                let result = await firebase.auth().signInWithPopup(provider);
                console.log('login onSignInFacebook', result.user.uid);
                let doc = await firebase.firestore().collection('users').doc(result.user.uid).get();
                if (!doc.exists || doc.data().username === undefined) {
                    await result.user.delete();
                    throw 'user does not exist';
                }
                this.running = false;
                this.cancel();
            } catch(err) {
                this.error = err;
                this.running = false;
            }
        },
        async onSignInGoogle() {
            this.running = true;
            try {
                var provider = new firebase.auth.GoogleAuthProvider();
                let result = await firebase.auth().signInWithPopup(provider);
                console.log('login onSignInGoogle', result.user.uid);
                let doc = await firebase.firestore().collection('users').doc(result.user.uid).get();

                if (!doc.exists || doc.data().username === undefined) {
                    await result.user.delete();
                    throw 'user does not exist';
                }
                this.running = false;
                this.cancel();
            } catch(err) {
                this.error = err;
                this.running = false;
            }
        }
    }
}
</script>