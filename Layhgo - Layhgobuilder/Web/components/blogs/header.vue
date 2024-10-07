<template>
    <v-hover v-slot:default="{ hover }">
        <v-card 
            flat
            :height="height"
            :class="{ 'on-hover': hover }"
            :to='localePath({ name: "blog-blogName", params: { blogName: blog.title.replace(/[^a-zA-Z0-9]/g,"_") }, query: { id: blog.id } })'>
            <v-img
                height="100%"
                gradient="to bottom left, rgba(255,255,255,.33), rgba(50,50,50,.4), rgba(50,50,50,.7)"
                :src="blog.image">
                <v-row no-gutters>
                    <v-col>
                        <v-list-item color="rgba(1, 0, 0, .4)" dark>
                            <v-list-item-title class="title">
                                {{localBlog.title}}
                            </v-list-item-title>
                        </v-list-item>
                    </v-col>
                </v-row>
                <v-row align="end" class="fill-height pb-12">
                    <v-col cols="12">
                        <v-list-item dark two-line>
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
                    </v-col>
                </v-row>
            </v-img>
        </v-card>
    </v-hover>
</template>

<script>
import Avatar from '@/shared/Cards/Avatar'
import Timestamp from '@/shared/Input/Timestamp'

export default {
    mounted() {
        this.$nextTick( () => {
            this.getAuthor();
        });
    },
    components: {
        'l-avatar': Avatar,
        'l-timestamp': Timestamp
    },
    props: {
        blog: {
            type: Object,
            required: true,
            default: function() {
                return {};
            }
        },
        height: {
            type: Number,
            required: false,
            default: 320
        }
    },
    computed: {
        author() {
            let author = Object.values(this.$store.getters['auth/builders']).find(builder => { return builder.id === this.blog.authorId; });
            return (author) ? author : {};
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
        getAuthor() {
            if (this.author.id) return;
            this.$store.dispatch('auth/getBuilderById', this.blog.authorId);
        }
    }
}
</script>

<style scoped>
.v-card {
  transition: opacity .4s ease-in-out;
}

.v-card:not(.on-hover) {
  opacity: 0.6;
 }
 </style>