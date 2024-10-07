<template>
    <v-container>
        <v-form ref='signupForm' @submit.prevent='onSignupEmail'>

            <v-row justify='center' align='center' no-gutters>
                <v-col cols='10'>

                    <v-row no-gutters>
                        <v-col cols='12'>
                            <v-text-field 
                                dense 
                                outlined 
                                :label='$t("display_name") + "*"'
                                autocomplete='off'
                                :counter='20'
                                :autofocus='true'
                                :rules='rules.displayName'
                                v-model='form.displayName'
                                @input='inputDisplay'>
                                <template v-slot:message='item'>
                                    {{ $t(item.message) }}
                                </template>
                            </v-text-field>
                        </v-col>

                        <v-col cols='12'>
                            <v-text-field 
                                dense 
                                outlined
                                type='email'
                                :label='$t("email") + "*"'
                                autocomplete='off'
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
                                counter
                                :label='$t("password") + "*"'
                                autocomplete='off'
                                :type="show ? 'text' : 'password'"
                                :append-icon="show ? 'mdi-eye' : 'mdi-eye-off'"
                                :rules='rules.password'
                                v-model='form.password'
                                @click:append="show = !show" >
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
                                :label='$t("confirm_password") + "*"'
                                :rules='rules.confirm'
                                v-model='form.confirm'>
                                <template v-slot:message='item'>
                                    {{ $t(item.message) }}
                                </template>
                            </v-text-field>
                        </v-col>

                        <v-col cols='12'>
                            <v-checkbox
                                name='terms'
                                v-model='form.terms'
                                :rules='rules.terms'
                                required>
                                <template v-slot:label>
                                    <l-terms @terms='changeTerms'></l-terms>
                                </template>
                                <template v-slot:message='item'>
                                    {{ $t(item.message) }}
                                </template>                                
                            </v-checkbox>
                        </v-col>

                        <v-col cols='12'>
                            <span class='caption red--text'>{{error}}</span>
                        </v-col>
                    </v-row>

                    <v-row>
                        <v-col cols='12' class='mb-5'>
                            <v-btn block outlined type='submit' :loading="running">
                                <v-icon left>mdi-login</v-icon>
                                {{ $t('sign_up') }}
                            </v-btn>
                        </v-col>
                    </v-row>
                </v-col>
            </v-row>
            <small>*{{ $t('required_field') }}</small>
        </v-form>

        <v-card flat color='provider'>
            <!-- <v-row no-gutters justify="center">
                <v-col cols='12' class="text-center">
                    <span class='font-weight-light'>{{ $t('or') }}</span>
                </v-col>
            </v-row>
            <v-row justify="center">
                <v-col cols='8'>
                    <v-btn block rounded depressed color='#3b5998' dark @click="onSignupFacebook">
                        <v-icon left>mdi-facebook</v-icon>
                        <span>Facebook</span>
                    </v-btn>
                </v-col>
                <v-col cols='8'>
                    <v-btn block rounded depressed @click="onSignupGoogle">
                        <v-icon left>mdi-google</v-icon>
                        <span>Google</span>
                    </v-btn>
                </v-col>
            </v-row> -->
        

            <v-row no-gutters class='mt-6'>
                <v-col cols='12'>
                    <v-btn block x-small outlined @click='$emit("login")'>
                        <v-icon left>mdi-arrow-left</v-icon>
                        {{ $t('have_account') }}
                    </v-btn>
                </v-col>
            </v-row>
        </v-card>
    </v-container>
</template>

<script>
import firebase from '@/plugins/firebase'
import Terms from './Terms'

export default {
    components: {
        'l-terms': Terms
    },
    data() {
        return {
            error: "",
            form: {},
            type: "email",
            running: false,
            occupied: false,
            show: false,
            rules: {
                displayName: [
                    v => !!v || 'rules.name_required',
                    v => (v && v.length <= 20) || 'rules.name_length',
                    v => !this.occupied || 'rules.name_occupied',
                    v => (v || '').indexOf(' ') < 0 || 'rules.name_spaces',
                    v => !/[^0-9a-zA-Z-_]/.test(v) || 'rules.name_special'
                ],
                email: [
                    v => (!!v || this.type !='email') || 'rules.email_required',
                    v => (/^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/.test(v) || this.type !='email') || 'rules.email_valid'
                ],
                password: [
                    v => (v && v.length >= 6  || this.type !='email') || 'rules.password_length'
                ],
                confirm: [
                    v => (v && v.length >= 6  || this.type !='email') || 'rules.password_length',
                    v => (v === this.form.password || this.type !='email') || 'rules.password_match'
                ],
                terms: [
                    v => (v && v === true) || 'rules.accept_terms'
                ]
            }
        }
    },
    methods: {
        cancel() {
            this.running = false;
            this.error = "";
            this.$refs.signupForm.reset();
            this.$emit('cancel');
        },
        open() {
            this.error = "";
        },
        inputDisplay() {
            this.occupied = false;
        },
        async checkUsername() {
            if (this.form.displayName !== undefined && this.form.displayName.length > 0) {
                let displayName = this.form.displayName.toLowerCase().trim().replace(/[^a-zA-Z0-9]/g,"_");
                console.log('signup checkUsername', displayName);
                let doc = await firebase.firestore().collection('usernames').doc(displayName).get();
                this.occupied = doc.exists;
            }
        },
        async onSignupEmail() {
            this.type = 'email';
            await this.checkUsername();

            if (!this.$refs.signupForm.validate()) {
                return;
            }

            this.running = true;
            try {
                let cred = await firebase.auth().createUserWithEmailAndPassword(this.form.email, this.form.password);
                await this.$store.dispatch('auth/signup', { ...cred.user, displayName: this.form.displayName });
                this.cancel();
            } catch( err ) {
                this.running = false;
                this.error = err.message;
                console.log(err);
            } 
        },
        async onSignupFacebook() {
            this.type = 'facebook';
            await this.checkUsername();
            if (!this.$refs.signupForm.validate()) {
                return;
            }

            this.running = true;
            try {
                var provider = new firebase.auth.FacebookAuthProvider();

                let result = await firebase.auth().signInWithPopup(provider);
                var credential = result.credential;
                var user = result.user;
                await this.$store.dispatch('auth/signup', { ...user, displayName: this.form.displayName });

                this.cancel();
            } catch(err) {
                this.running = false;
                this.error = err.message;
                console.log(err);
            }
        },
        async onSignupGoogle() {
            this.type = 'google';
            await this.checkUsername();
            if (!this.$refs.signupForm.validate()) {
                return;
			}

            this.running = true;
            try {
                var provider = new firebase.auth.GoogleAuthProvider();

                let result = await firebase.auth().signInWithPopup(provider);
                var credential = result.credential;
                var user = result.user;
                await this.$store.dispatch('auth/signup', { ...user, displayName: this.form.displayName });

                this.cancel();
            } catch(err) {
                this.running = false;
                this.error = err.message;
                console.log(err);
            }
        },
        changeTerms(state) {
            this.form.terms = state;
        }
    }
}
</script>