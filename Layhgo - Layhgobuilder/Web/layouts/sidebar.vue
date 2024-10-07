<template>
    <nav>
        <v-navigation-drawer v-model='drawer' app temporary>
            <v-app-bar>
                
            </v-app-bar>

            <l-profile></l-profile>
            <l-navigation></l-navigation>

            <template v-slot:append v-if='hasPermission'>
                <v-btn text tile block to='/admin/users'>
                    Admin Dashboard
                </v-btn>
            </template>
        </v-navigation-drawer>
    </nav>
</template>

<script>
import Profile from '@/components/Sidebar/Profile'
import Navigation from '@/components/Sidebar/Navigation'

export default {
    data() {
        return {
            drawer: false,
        }
    },
    created() {
        this.$bus.$on('onToggleSidebar', () => {
            this.drawer = !this.drawer;
        });
    },
    components: {
        'l-profile': Profile,
        'l-navigation': Navigation
    },
    computed: {
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        claims() {
            let claims = this.user.claims;
            return (claims) ? claims : {};
        },
        hasPermission() {
            return this.$store.getters['auth/hasPermission']([
                'krRmtPLsNOC4WzVdrpV7',
                'hRKVebDpx49GrNlktalm',
                'niMcHy7ggIAf93EcPPBa',
                'pLEVrfgbfX9KkLX3vPyx',
                'xp1qybL0DoeUTUMN32Uu',
                'X1kmNjE0ySJN7dwTB6ss',
                'LJkPWoWYgvCoNGezS6cC',

                'enbGnYeoeQ4FvmDvPOa2',
                'DZC958c04hQoVK8EzjW7',
                'Q2hQttT0cRY3ccErKbG6',
                'g9t3a7IuZ2FdQHnpvS9o',

                'ZXF4U8emDQNVRpJsDva3',
                'X09HqHqVK54LNfYaSNYL'
            ]);
        },
        roles() {
            return this.$store.getters['auth/roles'];
        }
    },
    methods: {
        toggleTheme() {
            this.$vuetify.theme.dark = !this.$vuetify.theme.dark;
            localStorage.setItem('theme', this.$vuetify.theme.dark);
            this.$bus.$emit('onToggleTheme');
        },
    }
}
</script>