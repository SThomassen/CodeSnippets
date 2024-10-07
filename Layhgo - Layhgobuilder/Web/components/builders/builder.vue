<template>
    <v-list-item :to='localePath(to)'>
        <v-list-item-avatar size='48px'>
            <l-avatar :builder='builder'></l-avatar>
        </v-list-item-avatar>

        <v-list-item-title>
            <span>{{builder.displayName}}</span>

            <span v-for='claim in claims' :key='claim'>
                <span class='ml-1'>
                    <l-role-icon :uid='claim'></l-role-icon>
                </span>
            </span>

            <span v-if='builder.incognito'>
                <v-tooltip right>
                    <template v-slot:activator='{ on }'>
                        <v-icon small v-on='on'>mdi-eye-off</v-icon>
                    </template>
                    {{ $t('private_account') }}
                </v-tooltip>
            </span>
        </v-list-item-title>

        <v-list-item-subtitle>
            <v-row no-gutters align='center' justify="end">
                <v-col cols='2'>
                    <v-tooltip right>
                        <template v-slot:activator='{ on }'>
                            <v-icon v-on='on'>mdi-briefcase</v-icon>
                        </template>
                        {{ $t('projects') }}
                    </v-tooltip>
                    <span class='ml-3'>{{ (stats.published > 0) ? stats.published : 0}}</span>
                </v-col>

                <v-col cols='2'>
                    <v-tooltip right>
                        <template v-slot:activator='{ on }'>
                            <v-icon v-on='on'>mdi-heart</v-icon>
                        </template>
                        {{ $t('votes') }}
                    </v-tooltip>
                    <span class='ml-3'>{{ (stats.votes > 0) ? stats.votes : 0}}</span>
                </v-col>

                <v-col cols='2'>
                    <v-tooltip right>
                        <template v-slot:activator='{ on }'>
                            <v-icon v-on='on'>mdi-account-group</v-icon>
                        </template>
                        {{ $t('views') }}
                    </v-tooltip>
                    <span class='ml-3'>{{ (stats.views > 0) ? stats.views : 0}}</span>
                </v-col>
            </v-row>
        </v-list-item-subtitle>

        <v-list-item-action v-if='hasPermission'>
            <l-menu :builder='builder'></l-menu>
        </v-list-item-action>
    </v-list-item>
</template>

<script>
import Avatar from '@/shared/Cards/Avatar'
import Role from '@/shared/Cards/Role'
import Menu from './menu'

export default {
    components: {
        'l-avatar': Avatar,
        'l-role-icon': Role,
        'l-menu': Menu
    },
    props: {
        builder: {
            type: Object,
            required: true,
            default: function () {
                return { }
            }
        }
    },
    computed: {
        to() {
            let username = (this.builder.username === undefined) ? "notfound" : this.builder.username;
            return { name: "builder-display", params: { display: username } };
        },
        stats() {
            return (this.builder['--stats--']) ? this.builder['--stats--'] : {};
        },
        roles() {
            let roles = this.$store.getters['auth/roles'];
            return (roles) ? roles : {};
        },
        claims() {
            if (this.builder.claims === undefined) {
                console.log(this.builder);
                return [];
            }
            let claims = Object.keys(this.builder.claims).filter(key => this.builder.claims[key]);
            return (claims) ? claims : [];
        },
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        hasPermission() {
            return this.$store.getters['auth/hasPermission']([]);
        },
    },
}
</script>