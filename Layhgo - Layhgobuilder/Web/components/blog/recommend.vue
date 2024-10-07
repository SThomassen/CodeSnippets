<template>
<v-container fluid>
    <v-row>
        <v-col cols='3' v-for='(blog,i) in recommend' :key='i'>
            <l-header :blog='blog' :height='192'></l-header>
        </v-col>
    </v-row>
    <v-list-item>
        <v-list-item-action>
            <v-btn small text v-if='previous' :to='localePath(previous)'>
                <v-icon left>mdi-chevron-left</v-icon>
                {{$t('previous_post')}}
            </v-btn>
        </v-list-item-action>

        <v-spacer></v-spacer>

        <v-list-item-action>
            <v-btn small text v-if='next' :to='localePath(next)'>
                {{$t('next_post')}}
                <v-icon right>mdi-chevron-right</v-icon>
            </v-btn>
        </v-list-item-action>
    </v-list-item>
</v-container>
</template>

<script>
import Header from '@/components/blogs/header'

export default {
    data() {
        return {
            today: new Date().getTime()
        }
    },
    mounted() {
        this.getBlogs()
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
    components: {
        'l-header': Header
    },
    computed: {
        blogs() {
            let blogs = this.$store.getters['blogs/blogs']; 
            let blogArray = (blogs) ? Object.values(blogs) : [];
            
            blogArray.sort( (a,b) => { return b.time - a.time; });
            return blogArray;
        },
        recommend() {
            let blogs = this.blogs
                .filter(blog => { return blog.time <= this.today && blog.id !== this.blog.id })
                .slice(0,4);
            return blogs;
        },
        tags() {
            return this.blog.tags;
        },
        index() {
            let index = this.blogs.findIndex(blog => blog.id === this.blog.id);
            return index;
        },
        previous() {           
            if (this.index < this.blogs.length) {
                let previous = this.blogs[this.index + 1];
                return (previous) ? { name: "blog-blogName", params: { blogName: previous.title }, query: { id: previous.id } } : null;
            }
            return null;
        },
        next() {
            if (this.index > 0) {
                let next = this.blogs[this.index - 1];
                return (next) ? { name: "blog-blogName", params: { blogName: next.title }, query: { id: next.id } } : null;
            }
            return null;
        }
    },
    methods: {
        async getBlogs() {
            if (this.blogs.length > 1) return;
            await this.$store.dispatch('blogs/getBlogs', { query: null, limit: 5 });
        }
    }
}
</script>