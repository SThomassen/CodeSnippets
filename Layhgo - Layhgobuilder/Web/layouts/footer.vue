<template>
    <v-footer app>
        <v-row no-gutters justify='start' align="center">
            <v-tooltip top>
                <template v-slot:activator='{ on }'>
                    <v-btn icon v-on='on' @click='toggleTheme'>
                        <v-icon>mdi-brightness-6</v-icon>
                    </v-btn>
                </template>
                <span>{{ $t("theme") }}</span>
            </v-tooltip>

            <v-menu top offset-y nudge-left="4" nudge-top="4">
                <template v-slot:activator="{ on, attrs }">
                    <v-btn
                        icon
                        v-bind="attrs"
                        v-on="on">
                        <v-avatar tile :size='16'>
                            <img
                                :src="currentFlag.icon"
                                :alt="currentLocale">
                        </v-avatar>
                    </v-btn>
                </template>
                <v-list>
                    <v-list-item
                        v-for="(item, index) in availableLocales"
                        :key="index"
                        :to='switchLocalePath(item.code)'>
                        <v-avatar tile :size='16'>
                            <img
                                :src="item.icon"
                                :alt="item.name">
                        </v-avatar>
                    </v-list-item>
                </v-list>
            </v-menu>

            <span class='caption ml-4'>{{version}}</span>
        </v-row>
        <v-row no-gutters justify='center'>
            <v-btn
                v-for="link in links"
                :key="link.name"
                :to='localePath(link.route)'
                text>
                <span class='caption' style="text-transform: none !important;">
                    {{ $t(link.name) }}
                </span>
            </v-btn>
        </v-row>
        <v-row no-gutters justify="end" align="center">
            <l-social></l-social>
        </v-row>
    </v-footer>
</template>

<script>
import Social from '@/shared/Media/Social'
import info from '@/config/info'

export default {
    data() {
        return {
            links: [
                { name: 'layhgobuilder', route: '/' },
                { name: 'projects', route: '/projects' },
                { name: 'instructions.title', route: '/instructions' },
                { name: 'blog', route: '/blog' },
                { name: 'contact', route: '/contact' }
            ],
            version: info.version,
        }
    },
    components: {
        'l-social': Social
    },
    computed: {
        currentLocale() {
            return this.$i18n.locale;
        },
        currentFlag() {
            return this.$i18n.locales.find(item => item.code === this.currentLocale);
        },
        availableLocales () {
            return this.$i18n.locales.filter(i => i.code !== this.currentLocale)
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