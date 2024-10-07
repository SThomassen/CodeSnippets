<template>
<span>
    <l-report ref='report' @submit='reportUser'></l-report>
    <v-menu 
        offset-x 
        transition="scale-transition"
        :close-on-content-click="false"
        v-if='hasPermission(["assign_roles", "manage_roles"])'
        v-model='menu'>
        <template v-slot:activator="{ on }">
            <v-btn icon small v-on='on' @click.prevent>
                <v-icon>mdi-dots-vertical</v-icon>
            </v-btn>
        </template>

        <v-list dense>
            <v-menu
                offset-x
                open-on-hover
                transition='slide-x-transition'
                :close-on-content-click="false">
                <template v-slot:activator="{ on }">
                    <v-list-item v-on='on'>
                        <v-icon left>mdi-shield</v-icon>
                        {{ $t('permissions.title')}}
                        <v-icon right>mdi-chevron-right</v-icon>
                    </v-list-item>
                </template>

                <v-list>
                    <v-list-item dense v-for='role in ordered' :key='role.id'>
                        <v-list-item-icon>
                            <v-icon :color='role.color'>
                                {{role.icon}}
                            </v-icon>
                        </v-list-item-icon>

                        <v-list-item-title>{{role.title}}</v-list-item-title>
                        
                        <v-list-item-action>
                            <v-checkbox
                                :disabled="role.index <= hasClaimsIndex"
                                v-model='claims[role.id]' 
                                @change='changeClaim(role.id, $event)'>
                            </v-checkbox>
                        </v-list-item-action>
                    </v-list-item>

                    <v-list-item dense>
                        <v-btn 
                            small 
                            block 
                            outlined 
                            :loading="loading"
                            :disabled="!changes"
                            @click='save'>
                            {{ $t('save') }}
                        </v-btn>
                    </v-list-item>
                </v-list>
            </v-menu>

            <v-list-item color='warning' @click.prevent='$refs.report.confirm(builder.username, builder)'>
                <v-icon left color='warning'>mdi-alert-circle</v-icon>
                {{ $t('warn') }}
            </v-list-item>

            <v-list-item color='error' @click.prevent='$refs.report.confirm(builder.username, builder)'>
                <v-icon left color='error'>mdi-cancel</v-icon>
                {{ $t('ban') }}
            </v-list-item>
            
        </v-list>
    </v-menu>
</span>
</template>

<script>
import Report from '@/shared/Dialog/Report'
import firebase from '@/plugins/firebase'

export default {
    data() {
        return {
            menu: false,
            loading: false,
            wrapper: {}
        }
    },
    components: {
        'l-report': Report
    },
    props: {
        builder: {
            type: Object,
            required: true,
            default: function() {
                return {};
            }
        }
    },
    watch: {
        menu(nv, ov) {
            this.wrapper = Object.assign({}, this.builder.claims);
        }
    },
    computed: {
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        roles() {
            let roles = this.$store.getters['auth/roles'];
            return (roles) ? roles : {};
        },
        ordered() {
            return Object.values(this.roles).sort( (a,b) => { return a.index - b.index; } );
        },
        claims() {
            return this.wrapper;
        },
        changes() {
            return JSON.stringify(this.builder.claims) !== JSON.stringify(this.wrapper);
        },
        hasClaimsIndex() {
            let max = 999;
            for (let key in this.user.claims) {
                if (this.user.claims[key]) {
                    let index = this.roles[key].index;
                    max = (index < max) ? index : max;
                }
            }
            return max;
        },
    },
    methods: {
        hasPermission(perms) {
            return this.$store.getters['auth/hasPermission'](perms);
        },
        changeClaim(key, nv) {
            if (!this.builder.claims[key]) {
                this.builder.claims[key] = false;
            }
            this.wrapper[key] = nv;
        },
        async save() {
            this.loading = true;
            try {
                let builder = Object.assign({}, this.builder);
                builder.claims = this.wrapper;
                await this.$store.dispatch('auth/updateUser', builder);
                this.wrapper = Object.assign({}, this.builder.claims);
            } catch(err) {
                console.log(err);
            }
            this.loading = false;
        },
        async reportUser(ticket) {
            let message = {
                userId: ticket.id,
                userName: ticket.username,
                reason: ticket.reason,
                description: ticket.description,
                moderatorId: this.user.id,
                moderatorName: this.user.username,
                moderatorTime: new firebase.firestore.Timestamp.now(),
            }
            await this.$store.dispatch('auth/warnUser', message);
        }
    }
}
</script>