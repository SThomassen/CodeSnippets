<template>
<div>
    <v-list-item v-if='user'>
        <v-list-item-avatar>
            <l-avatar :builder='user'></l-avatar>
        </v-list-item-avatar>
        <v-list-item-content>
            <v-textarea 
                :rows='1'
                :row-height="24"
                auto-grow
                :placeholder="$t('place_comment')"
                v-model='text'>
            </v-textarea>
        </v-list-item-content>
        <v-list-item-action>
            <v-btn outlined color='info' @click='submit'>{{$t('submit')}}</v-btn>
        </v-list-item-action>
    </v-list-item>

    <v-list>
        <v-list-item v-for='comment in comments' :key='comment.id'>
            <l-comment :comment='comment'></l-comment>
            <v-divider></v-divider>
        </v-list-item>
    </v-list>
</div>
</template>

<script>
import Comment from '@/components/comments/comment'
import Avatar from '@/shared/Cards/Avatar'

export default {
    data() {
        return {
            text: "",
        }
    },
    created() {
        this.getComments();
    },
    components: {
        'l-comment': Comment,
        'l-avatar': Avatar
    },
    props: {
        blog: {
            type: Object,
            required: true,
            default: function() {
                return {};
            }
        }
    },
    computed: {
        user() {
            return this.$store.getters['auth/user'];
        },
        comments() {
            let comments = this.$store.getters['comments/blog'];
            return comments.filter( item => { return item.blogId === this.blog.id }).sort(function(a,b) { return b.time - a.time; });
        },
        stats() {
            return (this.blog['--stats--']) ? this.blog['--stats--'] : {};
        }
    },
    methods: {
        submit() {
            let comment = {
                text: this.text,
                userId: this.user.id,
                blogId: this.blog.id,
                type: 'blogs'
            }
            this.$store.dispatch('comments/setComment', comment);
            this.text = "";
        },
        getComments() {
            this.$store.dispatch('comments/getCommentsByBlogId', this.$route.query.id);
        }
    },
}
</script>