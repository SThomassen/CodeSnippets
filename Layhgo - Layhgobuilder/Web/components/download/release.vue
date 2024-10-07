<template>
    <v-list-item link :ripple="false">
        <v-list-item-content>
            <v-list-item-title>
                {{ `Layhgobuilder ${release.id}` }}
                <v-chip class='ml-2' small :color='publishColor'>{{release.type}}</v-chip>
                <v-chip class='ml-2' small :color='productColor'>{{release.product}}</v-chip>
            </v-list-item-title>
            <v-list-item-subtitle>
                <l-timestamp :date='release.date' format="short"></l-timestamp><!--MMMM Do YYYY-->
            </v-list-item-subtitle>
        </v-list-item-content>

        <v-list-item-action >
            <v-layout fill-height justify-start>
                <v-col>
                    <template v-if='blog.id !== undefined'>
                        <v-btn 
                            depressed 
                            color="primary"
                            :to='localePath({ name: "blog-blogName", params: { blogName: blog.title.replace(/[^a-zA-Z0-9]/g,"_") }, query: { id: blog.id } })'>
                            {{$t('release_note')}}
                        </v-btn>
                    </template>
                </v-col>
            </v-layout>
        </v-list-item-action>
    </v-list-item>
</template>

<script>
import Timestamp from '@/shared/Input/Timestamp'
import info from '@/config/report'

export default {
    mounted() {
        this.getBlog();
    },
    props: {
        release: {
            type: Object,
            required: true,
            default: function() {
                return {}
            }
        }
    },
    components: {
        'l-timestamp': Timestamp
    },
    computed: {
        productColor() {
            return info.product.find(el => el.id === this.release.product).color;
        },
        publishColor() {
            return info.publish.find(el => el.id === this.release.type).color;
        },
        releases() {
            let releases = this.$store.getters['releases/versions'];
            return releases ? releases : {};
        },
        blog() {
            let blogs = this.$store.getters['blogs/blogs'];
            let blog = Object.values(blogs).find(blog => blog.id === this.release.blogId);
            return blog ? blog : {}
        }
    },
    methods: {
        getBlog() {
            if (this.blog.id !== undefined || this.release.blogId === undefined) {
                return;
            }

            this.$store.dispatch('blogs/getBlogById', this.release.blogId);
        }
    }
}
</script>