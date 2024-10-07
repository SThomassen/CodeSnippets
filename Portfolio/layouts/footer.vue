<template>
    <v-footer app>
        <v-row no-gutters justify="start">
            <!-- <v-tooltip top>
                <template v-slot:activator='{ on }'>
                    <v-btn icon v-on='on' @click="toggleTheme">
                        <v-icon>mdi-brightness-6</v-icon>
                    </v-btn>
                </template>
                <span>{{ $t("theme") }}</span>
            </v-tooltip> -->

            <v-btn 
                small
                plain
                v-for="(item, index) in allLocales" 
                :key="index"
                :to='switchLocalePath(item.code)'>
                {{item.code}}
            </v-btn>

            <!-- <v-btn 
                text
                plain
                v-for="(item, index) in allLocales" 
                :key="index"
                :to='switchLocalePath(item.code)'>
                {{item.code}}
                <v-avatar tile :size='16'>
                    <img
                        :src="item.icon"
                        :alt="item.name">
                </v-avatar>
            </v-btn> -->

            <!-- <v-menu top offset-y nudge-left="4" nudge-top="4">
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
            </v-menu> -->

            <v-spacer/>

            <v-tooltip top v-for='link in links' :key='link.title'>
                <template v-slot:activator='{ on }'>
                    <v-btn icon v-on='on' @click="open(link.url)">
                        <v-icon>{{link.icon}}</v-icon>
                    </v-btn>
                </template>
                {{$t(link.title)}}
            </v-tooltip>

        </v-row>

    </v-footer>
</template>

<script>
export default {
    data() {
        return {
            links: [
                {
                    title: 'email',
                    url: 'mailto:sjors2505@gmail.com',
                    icon: 'mdi-email'
                },
                {
                    title: 'linkedin',
                    url: 'linkedin.com/in/sjors-thomassen/',
                    icon: 'mdi-linkedin'
                },
                {
                    title: 'github',
                    url: 'https://github.com/SThomassen',
                    icon: 'mdi-github'
                }
            ],
        }
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
        },
        allLocales() {
            return this.$i18n.locales;
        }
    },
    methods: {
        toggleTheme() {
            this.$vuetify.theme.dark = !this.$vuetify.theme.dark;
            localStorage.setItem('theme', this.$vuetify.theme.dark);
            this.$bus.$emit('onToggleTheme');
        },
        open(url) {
            window.open(url, '_blank');
        }
    }
}
</script>