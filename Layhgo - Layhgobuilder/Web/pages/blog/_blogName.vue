<template>
<span>
    <v-container fluid>
        <v-img
            height="480"
            :position="`center ${blog.imageOffset}px`"
            :src='blog.image'>
            <div class="fill-height" style="position:absolute; width:100%; height:100%; top: 0; left: 0; background-image: linear-gradient(to top left, rgba(255,255,255,.33), rgba(255,255,255,.4), rgba(50,50,50,.7));"></div>
            <v-row class='ml-3 mt-2' justify='start'>
                <v-col cols='auto'>
                    <v-tooltip bottom>
                        <template v-slot:activator='{ on }'>
                            <v-btn dark small icon class='mr-3' v-on='on' @click.prevent='back'>
                                <v-icon>mdi-arrow-left</v-icon>
                            </v-btn>
                        </template>
                        <span>{{$t('go_to_blog')}}</span>
                    </v-tooltip>
                </v-col>

                <v-col cols='auto'>
                    <v-tooltip bottom v-if='hasPermission(["modify_post"])'>
                        <template v-slot:activator='{ on }'>
                            <v-btn dark small icon class='mr-3' v-on='on' @click.stop='edit'>
                                <v-icon>mdi-pencil</v-icon>
                            </v-btn>
                        </template>
                        {{$t('edit_blog')}}
                    </v-tooltip>
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
                        <v-chip label v-for='(tag,i) in blog.tags' :key='i'>
                            {{tag.text}}
                        </v-chip>
                    </v-chip-group>
                </v-list-item>

                <v-list-item>
                    <v-list-item-action>
                        <v-tooltip top>
                            <template v-slot:activator="{ on }">
                                <v-btn small icon @click.prevent='setVote' v-on="on">
                                    <v-icon large v-if='vote || auth'>mdi-heart</v-icon>
                                    <v-icon large v-else>mdi-heart-outline</v-icon>
                                </v-btn>
                            </template>
                            <span>{{$t('vote')}}</span>
                        </v-tooltip>
                    </v-list-item-action>
                    <span>{{votes}}</span>

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
                                    {{$t('share_facebook')}}
                                </v-tooltip>
                            </v-col>
                            <v-col>
                                <v-tooltip top>
                                    <template v-slot:activator='{ on }'>
                                        <v-btn icon v-on='on'>
                                            <v-icon>mdi-twitter</v-icon>
                                        </v-btn>
                                    </template>
                                    {{$t('share_twitter')}}
                                </v-tooltip>
                            </v-col>
                            <v-col>
                                <v-tooltip top>
                                    <template v-slot:activator='{ on }'>
                                        <v-btn icon v-on='on'>
                                            <v-icon>mdi-linkedin</v-icon>
                                        </v-btn>
                                    </template>
                                    {{$t('share_linkedin')}}
                                </v-tooltip>
                            </v-col>
                        </v-row>
                    </v-list-item-action>
                </v-list-item>

                <v-divider></v-divider>

                <l-recommend :blog='blog'></l-recommend>

                <!-- <l-comments :blog='blog'></l-comments> -->
                <l-comments type='blogs' :target='blog' :requireUser='false' :showComments='true'></l-comments>
            </v-col>
        </v-row>
    </v-container>
</span>
</template>

<script>
import Avatar from '@/shared/Cards/Avatar'
import Timestamp from '@/shared/Input/Timestamp'
// import Comments from '@/components/blog/comments'
import Recommend from '@/components/blog/recommend'
import Comments from '@/components/comments/comments'

export default {
    data() {
        return {
            vote: false,
            viewed: false
        }
    },
    mounted() {
        this.$nextTick( async () => {
            try {
                await this.getBlog();
                await this.getAuthor();
                await this.getVotes();
                await this.setView();
            } catch(err) {
                console.log(err);
            }
        })
    },
    components: {
        'l-avatar': Avatar,
        'l-timestamp': Timestamp,
        'l-comments': Comments,
        'l-recommend': Recommend,
    },
    computed: {
        blog() {
            let blog = this.$store.getters['blogs/blogs'][this.$route.query.id];
            return (blog) ? blog : {};
        },
        author() {
            let author = Object.values(this.$store.getters['auth/builders']).filter(builder => { return builder.id === this.blog.authorId; })[0];
            return (author) ? author : {};
        },
        auth() {
            return this.blog.authorId === this.user.id;
        },
        stats() {
            let stats = this.blog['--stats--'];
            return (stats) ? stats : {};
        },
        votes() {
            let votes = this.stats.votes;
            return (votes) ? votes : 0;
        },
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        localBlog() {
            let text = this.blog[this.$i18n.locale];
            if (text === undefined) {
                return this.blog;
            }
            return this.blog[this.$i18n.locale];
        },
    },
    methods: {
        hasPermission(perms) {
            return this.$store.getters['auth/hasPermission']([ "manage_blog", ...perms ]);
        },
        async getBlog() {
            if (this.blog.id) return;
            await this.$store.dispatch('blogs/getBlogById', this.$route.query.id);
        },
        async getAuthor() {
            if (this.author.id) return;
            await this.$store.dispatch('auth/getBuilderById', this.blog.authorId);
        },
        async getVotes() {
            this.vote = localStorage[`blog_${this.$route.query.id}_vote`]; // await this.$store.dispatch('blogs/getBlogVote', this.$route.query.id );
        },
        async setVote() {
            if (this.auth) return;

            localStorage.setItem(`blog_${this.$route.query.id}_vote`, !this.vote);
            try {
                await this.$store.dispatch('blogs/setBlogVote', { 
                    id: this.$route.query.id, 
                    vote: !this.vote,
                    votes: this.stats.votes
                });
                this.vote = !this.vote;
            } catch(error) {
                console.log(error);
            }
        },
        async setView() {
            let viewed = localStorage[`blog_${this.$route.query.id}_view`];
            if (viewed) return;

            localStorage.setItem(`blog_${this.$route.query.id}_view`, true);
            try {
                await this.$store.dispatch('blogs/setBlogView', this.$route.query.id);
            } catch(err) {
                console.log(err);
            }
        },
        edit() {
            let page = this.localePath({
                name: "blog-edit", 
                params: { 
                    blogName: this.blog.title 
                }, 
                query: { 
                    id: this.blog.id 
                } 
            });
            console.log(page);
            this.$router.push(page);
        },
        back() {
            this.$router.go(-1);
        }
    }
}
</script>