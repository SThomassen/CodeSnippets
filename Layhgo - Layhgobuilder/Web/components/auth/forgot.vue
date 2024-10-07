<template>
    <v-container fluid>
        <v-form ref='forgotForm' @submit.prevent='submit'>

            <v-row justify='center' align='center'>
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
                    </v-row>

                    <v-row justify="center">
                        <v-col cols='8'>
                            <v-btn block rounded depressed dark type='submit' :loading="sending">
                                <v-icon left>mdi-send</v-icon>
                                {{$t('submit')}}
                            </v-btn>
                        </v-col>
                        <v-col cols='8'>
                            <v-btn block rounded depressed @click="cancel">
                                <v-icon left>mdi-back</v-icon>
                                <span>{{$t('back')}}</span>
                            </v-btn>
                        </v-col>
                    </v-row>
                </v-col>
            </v-row>
        </v-form>
    </v-container>
</template>

<script>
import firebase from '@/plugins/firebase'

export default {
    data() {
        return {
            sending:false,
            form: {},
            rules: {
                email: [
                    v => !!v || 'rules.email_required',
                    v => /.+@.+/.test(v) || 'rules.email_valid'
                ],
            }
        }
    },
    methods: {
        cancel() {
            this.$emit('cancel');
        },
        async submit() {
            if (!this.$refs.forgotForm.validate()) {
                return;
            }
            this.sending = true;

            const callable = firebase.functions().httpsCallable('forgotPasswordEmail');
            await callable({ email: this.form.email });
            this.sending = false;
            this.$bus.$emit('onShowMessage', { message: this.$t('messages.reset_password_email_sent.message', { email: this.form.email }), timeout: -1 });
            this.cancel();
        }
    }
}
</script>