<template>
    <v-toolbar-items>
        <template v-for="group in menu">
            <v-menu
                left
                offset-y
                open-on-hover
                transition="slide-y-transition"
                :key="group.title"
                v-if='group.auth.length <= 0 || hasPermission(group.auth)'>
                <template v-slot:activator="{ on, attrs }">
                    <v-btn
                        text
                        v-bind="attrs"
                        v-on="on">
                        {{ $t(group.title) }}
                    </v-btn>
                </template>
                <v-list>
                    <template v-for="item in group.items">
                        <v-list-item
                            v-if='item.auth.length <= 0 || hasPermission(item.auth)'
                            :key="item.title"
                            :to="localePath(item.route)"
                            @click.prevent='item.function'>
                            <template>
                                <v-list-item-icon>
                                    <template v-if='item.customIcon !== undefined'>
                                        <v-avatar size='24'>
                                            <v-icon>{{`$vuetify.icons.${item.customIcon}`}}</v-icon>
                                        </v-avatar>
                                    </template>
                                    <template v-else>
                                        <v-icon>{{item.icon}}</v-icon>
                                    </template>
                                </v-list-item-icon>
                                <v-list-item-title>
                                    {{ $t(item.title) }}
                                </v-list-item-title>
                            </template>
                        </v-list-item>
                    </template>
                </v-list>
            </v-menu>
        </template>
    </v-toolbar-items>
</template>

<script>
export default {
    computed: {
        menu() {
            return [
                {
                    title: 'products.title',
                    auth: [ 'hidden' ],
                    items: [
                        { icon: 'mdi-rhombus', title: 'products.pro.title', route: '/products/pro', auth: [ ], function: () => {} },
                        { icon: 'mdi-rhombus-outline', title: 'products.basic.title', route: '/products/basic', auth: [ ], function: () => {} },
                        { customIcon: "fieldlab", title: 'products.fieldlab.title', route: '/products/fieldlab', auth: [ ], function: () => {} },
                        // { icon: 'mdi-rhombus-outline', title: 'web', route: '/products/web', function: () => {} },
                    ]
                },
                {
                    title: 'community',
                    auth: [],
                    items: [
                        { icon: 'mdi-rhombus-outline', title: 'projects', route: '/projects', auth: [], function: () => {} },
                        { icon: 'mdi-account-group', title: 'builders', route: '/builders', auth: [], function: () => { localStorage.setItem('builders', 1); } },
                        { icon: 'mdi-post-outline', title: 'blog', route: '/blog', auth: [], function: () => {} },
                        // { icon: 'mdi-trophy', title: 'Contests', route: '/contests' },
                        // { icon: 'mdi-calendar', title: 'Events', route: '/events' }
                    ]
                },
                {
                    title: 'support',
                    auth: [],
                    items: [
                        { icon: 'mdi-file-document-outline', title: 'terms_conditions', route: '/terms', auth: [], function: () => {} },
                        { icon: 'mdi-book-open-variant', title: 'instructions.title', route: '/instructions', auth: [], function: () => {} },
                        // { icon: 'mdi-help', title: 'faq', route: '/faq', auth: [], function: () => {} },
                        { icon: 'mdi-bug', title: 'reports.title', route: '/reports', auth: [], function: () => {} },
                        { icon: 'mdi-email', title: 'contact', route: '/contact', auth: [], function: () => {} }
                    ]
                }
            ]
        }
    },
    methods: {
        hasPermission(perms) {
            return this.$store.getters['auth/hasPermission']([ ...perms ]);
        },
    }
}
</script>