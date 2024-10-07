<template>
    <v-container>
        <v-row class='mt-2'>
            <v-col md='8' offset-md="2">

                <l-verify ref='verify' :user='user' @confirm='confirm'></l-verify>

                <span class='text-h2'>{{$t('settings')}}</span>
                <v-divider></v-divider>

                <!-- Account settings -->
                <v-list dense>

                    <v-list-item>
                        <v-divider></v-divider>
                        <span class='caption'>{{$t('account')}}</span>
                        <v-divider></v-divider>
                    </v-list-item>

                    <l-email ref='email' :user='user' :form='form' @error='error'></l-email>
                    
                    <v-list-item></v-list-item>
                    
                    <l-password ref='password' :user='user' :form='form' @error='error'></l-password>

                    <v-list-item>
                        <v-list-item-action>
                            <v-btn outlined color='info' @click="$refs.verify.open($t('messages.verify_account.title'), $t('messages.verify_account.message'), update);">
                                {{$t('save')}}
                            </v-btn>
                        </v-list-item-action>
                    </v-list-item>
                </v-list>

                <v-list dense>
                    <v-list-item>
                        <v-divider></v-divider>
                        <span class='caption'>{{$t('license')}}</span>
                        <v-divider></v-divider>
                    </v-list-item>

                    <v-list-item>
                        <l-license ref='license'></l-license>
                    </v-list-item>
                </v-list>
                
                <!-- Social Media -->
                <v-list>
                    <!-- <v-list-item>
                        <v-divider></v-divider>
                        <span class='caption'>{{$t('social_media')}}</span>
                        <v-divider></v-divider>
                    </v-list-item>

                    <v-list-item>
                        <l-connect name='facebook' icon='mdi-facebook' color='#3b5998' dark></l-connect>
                    </v-list-item>
                    <v-list-item>
                        <l-connect name='google' outlined></l-connect>
                    </v-list-item>
                    <v-list-item>
                        <l-connect name='twitter' color='#1DA1F2' dark></l-connect>
                    </v-list-item> -->

                    <v-list-item>
                        <v-divider></v-divider>
                        <span class='caption'>{{$t('danger_zone')}}</span>
                        <v-divider></v-divider>
                    </v-list-item>
                </v-list>

                
                <!-- Danger Zone -->
                <v-card outlined color='error'>
                    <v-list>
                        <v-list-item>
                            <v-row no-gutters align="center">
                                
                                <v-switch
                                    color='error'
                                    :label='$t("private_account")'
                                    :loading="incognitoLoad"
                                    :input-value='builder.incognito'
                                    @change="incognito">
                                </v-switch>

                                <v-spacer></v-spacer>

                                <v-btn small outlined color='error' @click="$refs.verify.open('messages.remove_account.title', 'message.remove_account.message',remove);">
                                    <v-icon>mdi-delete</v-icon>
                                    {{$t('delete_account')}}
                                </v-btn>
                            </v-row>
                        </v-list-item>
                    </v-list>
                </v-card>

            </v-col>
        </v-row>
    </v-container>
</template>

<script>
import firebase from '@/plugins/firebase'
import Connect from '@/shared/Media/Connect'

import Email from '@/components/settings/email'
import Password from '@/components/settings/password'
import License from '@/components/settings/license'
import Verify from '@/components/auth/verify'

export default {
    head() {
        return {
            title: `Layhgobuilder | Settings`,
        }
    },
    data() {
        return {
            form: {},
            timer: null,
            incognitoLoad: false
        }
    },
    components: {
        'l-connect':Connect,
        'l-email':Email,
        'l-password':Password,
        'l-license':License,
        'l-verify': Verify,
    },
    computed: {
        user() {
            let user = firebase.auth().currentUser;
            return (user) ? user : {};
        },
        builder() {
            let builder = this.$store.getters['auth/user'];
            return (builder) ? builder : {}
        }
    },
    methods: {
        confirm(func) {
            func();
        },
        update() {
            this.$refs.email.update();
            this.$refs.password.update();
        },
        remove() {
            this.$store.dispatch('auth/removeUser',this.builder.id);
        },
        async incognito(e) {
            this.incognitoLoad = true;
            await this.waitFor();

            if (e === this.builder.incognito) {
                this.incognitoLoad = false;
                return;
            }
            await this.$store.dispatch('auth/setIncognito', {
                id: this.builder.id,
                incognito: e
            });
            this.incognitoLoad = false;
            this.$bus.$emit('onShowMessage', { message: this.$t('private_changed') });
        },
        error(error) {
            this.$bus.$emit('onShowMessage', { message: error, color: "error" });

            this.$refs.email.reset();
            this.$refs.password.reset();
            this.form = {};
        },
        waitFor() {
            clearTimeout(this.timer);
            return new Promise( (resolve, reject) => {
                this.timer = setTimeout( () => {
                    resolve();
                }, 3000);
            })
        }
    }
}
</script>