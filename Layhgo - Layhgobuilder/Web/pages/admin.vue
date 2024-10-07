<template>
    <v-container fluid fill-height>
        <v-navigation-drawer 
            permanent
            mini-variant
            expand-on-hover
            v-if='$vuetify.breakpoint.mdAndUp'>
            <v-list>
                <span v-for='(item,i) in menu' :key='i'>
                    <v-list-item
                        ripple
                        :to='localePath(item.route)'>
                        <v-list-item-icon>
                            <v-icon>{{item.icon}}</v-icon>
                        </v-list-item-icon>
                        <v-list-item-title>
                            {{$t(item.title)}}
                        </v-list-item-title>
                    </v-list-item>
                </span>
            </v-list>
        </v-navigation-drawer>

        <v-bottom-navigation v-else>
            <v-btn
                v-for='(item,i) in menu'
                :key='i'
                :to='localePath(item.route)'>
                <span>{{item.title}}</span>
                <v-icon>{{item.icon}}</v-icon>
            </v-btn>
        </v-bottom-navigation>

        <v-main style='height:100%; padding: 0px'>
            <v-container fluid>
                <nuxt-child></nuxt-child>
            </v-container>
        </v-main>

    </v-container>
</template>

<script>
export default {
    data() {
        return {
            menu: [
                { 
                    icon: 'mdi-account-group', 
                    title: 'users', 
                    route: '/admin/users', 
                    permissions: [
                        "refactor_user", 
                        "download_user", 
                        "manage_users"
                    ] 
                },
                { 
                    icon: 'mdi-folder', 
                    title: 'projects', 
                    route: '/admin/projects', 
                    permissions: [] 
                },
                { 
                    icon: 'mdi-security', 
                    title: 'permissions.title', 
                    route: '/admin/roles', 
                    permissions: [
                        "manage_roles", 
                        "create_roles", 
                        "remove_roles"
                    ] 
                },
                { 
                    icon: 'mdi-post-outline', 
                    title: 'blog', 
                    route: '/admin/blog', 
                    permissions: [] 
                },
                { 
                    icon: 'mdi-key', 
                    title: 'licenses', 
                    route: '/admin/licenses', 
                    permissions: [] 
                },
                { 
                    icon: 'mdi-application-outline', 
                    title: 'releases.title', 
                    route: '/admin/releases', 
                    permissions: [] 
                },
                {
                    icon: 'mdi-ticket-outline',
                    title: 'tickets',
                    route: '/admin/tickets',
                    permissions: []
                }
            ],
        }
    },
    head() {
        return {
            title: "Layhgobuilder | Admin"
        }
    },
    middleware({app, store, redirect}) {
        if (!store.getters['auth/hasPermission'](
            [
                'administrator',
                "manage_roles",
                "create_roles",
                "modify_roles",
                "remove_roles",
                "assign_roles",
                "manage_users",
                "warn_user",
                "ban_user",
                "download_user",
                // "change_displayname",
                "bypass_incognito",
                "refactor_user",
                "manage_projects",
                "import_project",
                "export_project",
                "share_project",
                "showcase_project",
                "bypass_unpublished",
                "refactor_project",
                "manage_blog",
                "create_post",
                "modify_post",
                "remove_post",
                "set_popup",
                "download_post"
            ]
            )) {
            return redirect(app.localePath('/')); 
        }
    },
    methods: {
        hasPermission(perms) {
            return this.$store.getters['auth/hasPermission'](perms);
        }
    }
}
</script>