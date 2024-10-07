<template>
    <v-container fluid>
        <v-form ref='license'>
            <v-row no-gutters class='mt-4'>
                <v-col>
                    <v-text-field 
                        dense 
                        outlined 
                        :label='$t("releases.license_key")' 
                        :rules='rules.email_new'
                        v-model='form.license'>
                    </v-text-field>
                </v-col>
                <v-col cols='auto' class='ml-4'>
                    <v-btn 
                        outlined 
                        :loading='loading' 
                        @click.prevent='activate'>
                        {{$t("activate")}}
                    </v-btn>
                </v-col>
                <v-col cols='12'>
                    <v-btn x-small block text @click.prevent="$emit('request')">
                        {{$t("products.request_license")}}
                    </v-btn>
                </v-col>
            </v-row>
        </v-form>
    </v-container>
</template>

<script>
export default {
    data() {
        return {
            loading: false,
            form: {},
            rules: {
                email_new: [
                    v => (v && v.length >= 6) || 'Min 6 characters',
                ],
            }
        }
    },
    computed: {
        user() {
            let user = this.$store.getters['auth/user'];
            return user ? user : {};
        }
    },
    methods: {
        cancel() {
            this.$emit('cancel');
        },
        open() {
            
        },
        async activate() {
            if (!this.$refs.license.validate())
                return;

            try {
                this.loading = true;
                await this.$store.dispatch('licenses/submitLicense', { key: this.form.license, userId: this.user.id, username: this.user.username });
                this.loading = false;
                this.form.licenses = "";
                this.cancel();
            } catch(err) {
                console.log(err);
            }
        }
    }
}
</script>