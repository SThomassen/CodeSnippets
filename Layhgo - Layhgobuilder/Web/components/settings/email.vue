<template>
    <v-form ref='email'>
        <v-list-item>
            <v-text-field
                dense
                outlined
                :label='$t("new_email")' 
                :rules='rules.email_new'
                :placeholder='user.email'
                v-model='form.new_email'>
            </v-text-field>
        </v-list-item>

        <v-list-item>
            <v-text-field
                dense
                outlined
                :label='$t("confirm_new_email")' 
                :rules='rules.email_confirm'
                v-model='form.confirm_email'>
            </v-text-field>
        </v-list-item>
    </v-form>
</template>

<script>
import firebase from '@/plugins/firebase'

export default {
    data() {
        return {
            rules: {
                email_new: [
                     v => (/.+@.+/.test(v) || !v) || 'E-mail must be valid'
                ],
                email_confirm: [
                    v => (/.+@.+/.test(v) || !v) || 'E-mail must be valid',
                    v => (v === this.form.new_email) || 'E-mails do not match'
                ],
            }
        }
    },
    props: {
        user: {
            type: Object,
            required: true,
            default: function() {
                return {}
            }
        },
        form: {
            type: Object,
            required: true,
            default: function() {
                return {}
            }
        }
    },
    methods: {
        async update() {
            if (!this.$refs.email.validate() || 
                this.form.new_email === undefined ||
                this.form.new_email?.length <= 0) return;

            try {
                let user = firebase.auth().currentUser;
                let result = await user.updateEmail(this.form.new_email);
                await this.$store.dispatch('auth/updateUser', { email: this.form.new_email });
                this.$refs.email.reset();
            } catch(err) {
                this.$emit('error', err.message);
            }
        },
        reset() {
            this.$refs.email.reset();
        }
    }
}
</script>