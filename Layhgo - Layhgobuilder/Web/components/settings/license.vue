<template>
    <v-container>
        <v-form ref='license'>
            <v-row>
                <v-spacer/>
                <v-col>
                    <v-text-field
                        dense
                        outlined
                        :label='$t("releases.license_key")' 
                        :rules='rules.email_new'
                        v-model='form.license'>
                    </v-text-field>
                </v-col>
                <v-col cols='auto'>
                    <v-btn
                        dense
                        outlined
                        :loading="loading"
                        @click.prevent='activate'>
                        {{$t('releases.activate')}}
                    </v-btn>
                </v-col>
            </v-row>
        </v-form>

        <v-list>
            <template v-for='license in licenses'>
                <v-card :key='license.id' outlined>
                    <v-list-item>
                        <v-list-item-title>
                            <span v-for='(value,key) in license.roles' :key='key'>
                                <l-role v-if='value' :uid='key'></l-role>
                            </span>
                        </v-list-item-title>

                        <v-list-item-subtitle>
                            <template v-if='license.expires'>
                                <span>{{$t("releases.expires")}}: </span>
                                <l-timestamp ref='expireDate' :label='$t("releases.expires")' :date="license.expires"></l-timestamp>
                            </template>
                        </v-list-item-subtitle> 

                        <v-list-item-subtitle>
                            <span>{{$t("claimed")}}: </span>
                            <l-timestamp ref='claimDate' :label='$t("claimed")' :date="license.claimed"></l-timestamp>
                        </v-list-item-subtitle>
                        
                        <!-- <v-list-item-action>
                            <v-btn icon>
                                <v-icon>mdi-delete</v-icon>
                            </v-btn>
                        </v-list-item-action> -->
                    </v-list-item>
                </v-card>
            </template>
        </v-list>
    </v-container>
</template>

<script>
import Role from '@/components/admin/users/role'
import Timestamp from '@/shared/Input/Timestamp'

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
    created() {
        if (this.user.id !== undefined) {
            this.getLicenses();
        }
    },
    components: {
        'l-role': Role,
        'l-timestamp': Timestamp
    },
    watch: {
        user(nv) {
            if (this.licenses.length <= 0)
            {
                this.getLicenses();
            }
        }   
    },
    computed: {
        licenses() {
            let licenses = this.$store.getters['licenses/licenses'];
            return Object.values(licenses).filter(el => el.userId === this.user.id && !el.invoked);
        },
        user() {
            let user = this.$store.getters['auth/user'];
            return user ? user : {};
        }
    },
    methods: {
        async getLicenses() {
            try {
                await this.$store.dispatch('licenses/getLicensesByUserId', this.user.id);
                
            } catch(err) {
                console.log(err);
            }
        },
        async activate() {
            if (!this.$refs.license.validate())
                return;

            try {
                this.loading = true;
                await this.$store.dispatch('licenses/submitLicense', { key: this.form.license, userId: this.user.id, username: this.user.username });
                this.loading = false;
                this.form.licenses = "";
            } catch(err) {
                console.log(err);
            }
        }
    }
}
</script>