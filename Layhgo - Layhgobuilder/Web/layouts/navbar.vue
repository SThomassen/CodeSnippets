<template>
    <nav>
        <v-app-bar app>
            <v-toolbar-title headline text-uppercase>
                <router-link :to="localePath('/')" tag='span' style='cursor: pointer'>
                    <v-tooltip bottom>
                        <template v-slot:activator='{ on }'>
                            <div v-on='on'>
                                <span class='display-1 font-weight-light'>Layhgobuilder</span>
                                <span class='caption font-weight-light'>beta</span>
                            </div>
                        </template>
                        <span>{{ $t("go_to_home") }}</span>
                    </v-tooltip>
                </router-link>
            </v-toolbar-title>

            <v-spacer></v-spacer>

            <l-navigation></l-navigation>
            <l-profile class='mr-2'></l-profile>
        </v-app-bar>
    </nav>
</template>

<script>
import Navigation from '@/components/Sidebar/Navigation'
import Profile from '@/components/Sidebar/Profile'
import Avatar from '@/shared/Cards/Avatar'

export default {
    
    components: {
        'l-navigation': Navigation,
        'l-profile': Profile,
        'l-avatar': Avatar
    },
    computed: {
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        authenticated() {
            let user = this.$store.getters['auth/user'];
            return user !== null && user !== undefined;
        },
    }, 
    methods: {
        toggleSidebar() {
            this.$bus.$emit('onToggleSidebar');
        }
    }
}
</script>

<style lang="scss">
.custom-icon {
  fill: currentColor;
}

.custom-icon-background {
  background: currentColor;
}
</style>