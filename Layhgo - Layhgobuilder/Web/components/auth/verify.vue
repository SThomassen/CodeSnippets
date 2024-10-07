<template>
    <v-dialog 
        max-width='600px'
        v-model='dialog'
        transition="dialog-bottom-transition"
        @keydown.esc="cancel"
        @click:outside='cancel'>
        <v-card>

            <v-app-bar flat>
                <v-toolbar-title>{{title}}</v-toolbar-title>
            </v-app-bar>

            <v-container fluid class='text-center'>
                <v-card-subtitle>{{message}}</v-card-subtitle>
                <v-window v-model="tab">
                    <v-window-item :value='0'>
                        <v-row no-gutters justify="center">
                            <v-col 
                                cols='8'
                                class='mb-3'
                                v-if='providers["password"]'>
                                <v-btn
                                    dark
                                    block 
                                    rounded 
                                    depressed 
                                    @click="tab = 1">
                                    <v-icon left>mdi-lock</v-icon>
                                    {{$t('password')}}
                                </v-btn>
                            </v-col>
                            <v-col 
                                cols='8'
                                class='mb-3'
                                v-if='providers["facebook.com"]'>
                                <v-btn 
                                    dark
                                    block 
                                    rounded 
                                    depressed 
                                    color='#3b5998'  
                                    @click="onSignInFacebook">
                                    <v-icon left>mdi-facebook</v-icon>
                                    Facebook
                                </v-btn>
                            </v-col>
                            <v-col 
                                cols='8'
                                class='mb-3'
                                v-if='providers["google.com"]'>
                                <v-btn 
                                    light
                                    block 
                                    rounded 
                                    depressed 
                                    @click="onSignInGoogle">
                                    <v-icon left>mdi-google</v-icon>
                                    <span>Google</span>
                                </v-btn>
                            </v-col>
                        </v-row>
                    </v-window-item>

                    <v-window-item :value='1'>
                         <v-row no-gutters justify="center">
                            <v-col 
                                cols='8'
                                class='mb-3'
                                v-if='providers["password"]'>
                                <v-form ref='currentPassword'>
                                    <v-text-field
                                        dense
                                        outlined
                                        persistent-hint
                                        autocomplete='off'
                                        :rules='rules.password'
                                        :label='$t("current_password")'
                                        :hint='$t("current_password_hint")'
                                        :type="show ? 'text' : 'password'"
                                        :append-icon="show ? 'mdi-eye' : 'mdi-eye-off'"
                                        @click:append="show = !show"
                                        @input='valid = true'
                                        v-model='form.password'>
                                        <template v-slot:message='item'>
                                            {{ $t(item.message) }}
                                        </template>
                                    </v-text-field>
                                </v-form>

                                <v-btn 
                                    block 
                                    outlined 
                                    class='mt-3'
                                    :loading='checking'
                                    @click="onSignInPassword">
                                    {{$t('verify')}}
                                </v-btn>
                            </v-col>
                         </v-row>
                    </v-window-item>
                </v-window>
            </v-container>
        </v-card>
    </v-dialog>
</template>

<script>
import firebase from '@/plugins/firebase'

export default {
    data() {
        return {
            tab: 0,
            dialog: false,
            show: false,
            checking: false,
            form: {},
            providers: {},
            rules: {
                password: [
                    v => (v && v.length >= 6) || 'rules.password_length',
                ],
            },
            title: "",
            message: "",
            callback: null
        }
    },
    props: {
        user: {
            type: Object,
            required: true,
            default: function() {
                return {};
            }
        },
    },
    methods: {
        open(title, message, callback) {
            this.dialog = true;
            this.title = title;
            this. message = message;

            this.callback = callback;
            let auth = Object.assign({}, this.user);

            if (auth.providerData === undefined) {
                auth = firebase.auth().currentUser;
            }
            auth.providerData.forEach( profile => {
                this.providers[profile.providerId] = true;
            });
        },
        cancel() {
            this.dialog = false;
            this.tab = 0;
            
            if (this.$refs.currentPassword) {
                this.$refs.currentPassword.reset();
            }
        },
        confirm() {
            this.checking = false;
            this.cancel();
            this.$emit('confirm', this.callback);
        },
        async onSignInPassword() {
            if (!this.$refs.currentPassword.validate()) return;

            try {
                this.checking = true;
                // let user = firebase.auth().currentUser;
                let credential = firebase.auth.EmailAuthProvider.credential(
                    this.user.email, 
                    this.form.password
                );
                
                let result = await this.user.reauthenticateWithCredential(credential);
                this.confirm();
            } catch(err) {
                console.log(err);
            }
        },
        async onSignInFacebook() {
            try {
                this.checking = true;
                // let user = firebase.auth().currentUser;
                let provider = new firebase.auth.FacebookAuthProvider();
                let result = await this.user.reauthenticateWithPopup(provider);
                this.confirm();
            } catch(err) {
                console.log(err);
            }
        },
        async onSignInGoogle() {
            try {
                this.checking = true;
                // let user = firebase.auth().currentUser;
                let provider = new firebase.auth.GoogleAuthProvider();
                let result = await this.user.reauthenticateWithPopup(provider);
                this.confirm();
            } catch(err) {
                console.log(err);
            }
        }
    }
}
</script>