<template>
    <v-toolbar-items>
        <div style="width:256px">
            <template v-if='user'>
                <v-menu
                    left
                    offset-y
                    open-on-hover
                    transition="slide-y-transition">
                    <template v-slot:activator="{ on, attrs }">
                        <v-list-item
                            v-bind="attrs"
                            v-on="on"
                            v-if='user'
                            :to='localePath(to)'>
                            <v-list-item-content>
                                <v-list-item-title>
                                    {{user.displayName}}
                                </v-list-item-title>
                                <v-list-item-subtitle>
                                    {{user.email}}
                                </v-list-item-subtitle>
                            </v-list-item-content>

                            <v-list-item-avatar>
                                <l-avatar :builder='user'></l-avatar>
                            </v-list-item-avatar>                
                        </v-list-item>
                    </template>
                    <v-list>
                        <template v-for="item in menu">
                            <v-list-item
                                v-if='!item.auth || (item.auth && hasPermission(item.permissions))'
                                :key='item.title'
                                :to="localePath(item.route)">
                                <v-list-item-icon>
                                    <v-icon>{{item.icon}}</v-icon>
                                </v-list-item-icon>
                                <v-list-item-title>
                                    {{ $t(item.title) }}
                                </v-list-item-title>
                            </v-list-item>
                        </template>

                        <v-list-item @click.prevent='logout'>
                            <v-list-item-icon>
                                <v-icon>mdi-logout-variant</v-icon>
                            </v-list-item-icon>
                            <v-list-item-title>
                                {{ $t("log_out") }}
                            </v-list-item-title>
                        </v-list-item>
                    </v-list>
                </v-menu>
            </template>

            <template v-else>
                <v-layout fill-height align-center>
                    <v-btn text @click.prevent='login'>
                        <v-icon left>mdi-login-variant</v-icon>
                        {{ $t("log_in") }}
                    </v-btn>
                    <v-btn text @click.prevent='signup'>
                        <v-icon left>mdi-account-plus</v-icon>
                        {{ $t("sign_up") }}
                    </v-btn>
                </v-layout>
            </template>
        </div>
    </v-toolbar-items>
</template>

<script>
import Avatar from '@/shared/Cards/Avatar'

export default {
    components: {
        'l-avatar': Avatar
    },
    computed: {
        user() {
            return this.$store.getters['auth/user'];
        },
        to() {
            let username = (this.user.username === undefined) ? "notfound" : this.user.username;
            return { name: "builder-display", params: { display: username } };
        },
        settings() {
            return { name: "settings"};
        },
        menu() {
            return [
                {
                    auth: false,
                    icon: 'mdi-briefcase', 
                    title: 'my_projects', 
                    route: { 
                        name: 'my_projects'
                    }
                },
                // {
                //     auth: false,
                //     icon: 'mdi-download', 
                //     title: 'download', 
                //     route: { 
                //         name: 'download'
                //     }
                // },
                {
                    auth: false,
                    icon: 'mdi-cog', 
                    title: 'settings', 
                    route: { 
                        name: 'settings'
                    }
                },
                {
                    auth: true,
                    icon: 'mdi-archive', 
                    title: 'administrator', 
                    route: '/admin/users',
                    permissions: [
                        "manage_roles",
                        "create_roles",
                        "remove_roles",
                        "assign_roles",
                        "manage_users",
                        "manage_projects"
                    ]
                },
            ]
        }
    },
    methods: {
        hasPermission(perms) {
            return this.$store.getters['auth/hasPermission'](perms);
        },
        login() {
            this.$bus.$emit('onOpenLogin');
        },
        signup() {
            this.$bus.$emit('onOpenSignup');
        },
        logout() {
            this.$store.dispatch('auth/logout')
            .then( result => {
                this.$router.push( { path: this.localePath('/') }, success => {}, error => {});
            }, error => {  
                console.log(error);
            })
        }
    }
}
</script>