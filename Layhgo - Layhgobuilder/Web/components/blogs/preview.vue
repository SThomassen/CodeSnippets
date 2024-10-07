<template>
<span>
    <v-container fluid>
        <v-img
            height="320"
            gradient="to top left, rgba(255,255,255,.33), rgba(255,255,255,.4), rgba(50,50,50,.7)"
            :src='blog.image'>
            <v-row no-gutters>
                <v-col>
                    <v-list-item :dark='blog.image !== undefined'>
                        <v-list-item-action>
                            <v-tooltip right>
                                <template v-slot:activator='{ on }'>
                                    <v-btn small icon v-on='on' @click='$emit("close")'>
                                        <v-icon>mdi-close</v-icon>
                                    </v-btn>
                                </template>
                                Close preview
                            </v-tooltip>
                        </v-list-item-action>
                    </v-list-item>
                </v-col>
            </v-row>
        </v-img>
    </v-container>
    <v-container>
        <v-row no-gutters>
            <v-col cols='12' md='8' offset-md='2'>
                <span class='display-1'>{{localBlog.title}}</span>
                <v-divider></v-divider>

                <v-list-item two-line>
                    <v-list-item-avatar>
                        <l-avatar :builder='author'></l-avatar>
                    </v-list-item-avatar>
                    <v-list-item-content>
                        <v-list-item-subtitle>
                            {{author.displayName}}
                        </v-list-item-subtitle>
                        <v-list-item-subtitle>
                            <l-timestamp :date='blog.published' format='short'></l-timestamp><!--D MMMM YYYY-->
                        </v-list-item-subtitle>
                    </v-list-item-content>
                </v-list-item>

                <span v-html='localBlog.message'></span>

                <v-list-item>
                    <v-chip-group>
                        <v-chip v-for='(tag,i) in blog.tags' :key='i'>
                            {{tag.text}}
                        </v-chip>
                    </v-chip-group>
                </v-list-item>

                <v-list-item>
                    <v-list-item-action>
                        <v-tooltip top>
                            <template v-slot:activator="{ on }">
                                <v-btn icon v-on="on">
                                    <v-icon large>mdi-heart</v-icon>
                                </v-btn>
                            </template>
                            <span>Vote</span>
                        </v-tooltip>
                    </v-list-item-action>
                    <span>{{blog.votes}}</span>

                    <v-spacer></v-spacer>

                    <v-list-item-action>
                        <v-row>
                            <v-col>
                                <v-tooltip top>
                                    <template v-slot:activator='{ on }'>
                                        <v-btn icon v-on='on'>
                                            <v-icon>mdi-facebook</v-icon>
                                        </v-btn>
                                    </template>
                                    Share on Facebook
                                </v-tooltip>
                            </v-col>
                            <v-col>
                                <v-tooltip top>
                                    <template v-slot:activator='{ on }'>
                                        <v-btn icon v-on='on'>
                                            <v-icon>mdi-twitter</v-icon>
                                        </v-btn>
                                    </template>
                                    Share on Twitter
                                </v-tooltip>
                            </v-col>
                            <v-col>
                                <v-tooltip top>
                                    <template v-slot:activator='{ on }'>
                                        <v-btn icon v-on='on'>
                                            <v-icon>mdi-linkedin</v-icon>
                                        </v-btn>
                                    </template>
                                    Share on LinkedIn
                                </v-tooltip>
                            </v-col>
                        </v-row>
                    </v-list-item-action>
                </v-list-item>

                <v-divider></v-divider>
            </v-col>
        </v-row>
    </v-container>
</span>
</template>

<script>
import Avatar from '@/shared/Cards/Avatar'
import Timestamp from '@/shared/Input/Timestamp'

export default {
    data() {
        return {
            vote: false,
        }
    },
    props: {
        blog: {
            type: Object,
            required: true,
            default: function() {
                return {};
            }
        },
        author: {
            type: Object,
            required: true,
            default: function() {
                return {};
            }
        }
    },
    components: {
        'l-avatar': Avatar,
        'l-timestamp': Timestamp
    },
    computed: {
        localBlog() {
            let text = this.blog[this.$i18n.locale];
            if (text === undefined) {
                return this.blog;
            }
            return this.blog[this.$i18n.locale];
        },
    }
}
</script>