<template>
     <v-form ref='password'>
        <v-list-item>
            <v-text-field
                dense
                outlined
                :label='$t("new_password")'
                autocomplete='off'
                :rules='rules.password_new'
                :type="showPassword ? 'text' : 'password'"
                :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                @click:append="showPassword = !showPassword"
                v-model='form.new_password'>
            </v-text-field>
        </v-list-item>

        <v-list-item>
            <v-text-field
                dense
                outlined
                :label='$t("confirm_new_password")'
                autocomplete='off'
                :hint='$t("confirm_change_password")'
                type="password"
                :rules='rules.password_confirm'
                v-model='form.confirm_password'>
            </v-text-field>
        </v-list-item>
    </v-form>
</template>

<script>
import firebase from '@/plugins/firebase'

export default {
     data() {
        return {
            // form: {},
            showCurrent: false,
            showPassword: false,
            showConfirm: false,
            rules: {
                password_current: [
                    v => (v && v.length >= 6) || 'Min 6 characters',
                    v => (v && this.validCredentials) || 'Invalid password.'
                ],
                password_new: [
                    v => (v && v.length >= 6 || !v) || 'Min 6 characters'
                ],
                password_confirm: [
                    v => (v && v.length >= 6 || !v) || 'Min 6 characters',
                    v => (v === this.form.new_password) || 'Passwords do not match'
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
            if (!this.$refs.password.validate() || 
                this.form.new_password === undefined || 
                this.form.new_password.length <= 0) return;

            try {
                let user = firebase.auth().currentUser;
                let result = await user.updatePassword(this.form.new_password);
                this.$refs.password.reset();
            } catch(err) {
                this.$emit('error', err.message);
            }
        },
        reset() {
            this.$refs.password.reset();
        }
    }
}
</script>