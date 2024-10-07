<template>
    <v-container>
        <l-scrolltotop :y='offsetTop'></l-scrolltotop>

        <template>
            <v-row class='mt-2' v-scroll='onScroll'>
                <v-col>
                    <v-row align='center' class='mb-2'>
                        <v-col cols='5'>
                            <span class='text-h2'>{{ $t('blog') }}</span>
                        </v-col>

                        <v-spacer></v-spacer>

                        <v-col cols='auto'>
                            <v-tooltip right v-if='hasPermission(["create_post"])'> <!--hasPermission(["Fgh64Zxle4cCFfZtR4Hn"])-->
                                <template v-slot:activator='{ on }'>
                                    <v-btn outlined v-on='on' :to='localePath({ name: "blog-create" })'>
                                        {{$t('create_blog')}}
                                        <v-icon>mdi-plus</v-icon>
                                    </v-btn>
                                </template>
                                {{ $t('create_blog') }}
                            </v-tooltip>
                            <!-- <l-social></l-social> -->
                        </v-col>

                        <v-col class="d-flex" cols='5' md='4' lg='3' xl='2'>
                            <v-combobox
                                v-model="filter"
                                dense
                                outlined
                                hide-selected
                                persistent-hint
                                :label="$t('filter')"
                                :items="filters"
                                :search-input.sync="filterSearch">
                                <template v-slot:selection='{item}'>
                                    {{ $t(item) }}
                                </template>
                                <template v-slot:item='{item}'>
                                    {{ $t(item) }}
                                </template>
                                <!-- <template v-slot:no-data>
                                    <v-list-item>
                                    <v-list-item-content>
                                        <v-list-item-title>
                                        No results matching "<strong>{{ search }}</strong>". Press <kbd>enter</kbd> to create a new one
                                        </v-list-item-title>
                                    </v-list-item-content>
                                    </v-list-item>
                                </template> -->
                            </v-combobox>
                        </v-col>
                        
                        <v-col class="d-flex" cols='5' md='4' lg='3' xl='2'>
                            <v-text-field
                                dense
                                outlined
                                clearable
                                hide-details
                                :label='$t("search")'
                                class='pr-2 pl-2'
                                append-icon="mdi-magnify"
                                v-model='search'
                                @click:append="searchBlog"
                                @click:clear="searching = ''">
                            </v-text-field>
                        </v-col>
                    </v-row>

                    <v-divider></v-divider>

                    <v-row class='pa-2 mt-2'>
                        <v-col cols='12' v-for='blog in header' :key='blog.id'>
                            <l-header :blog='blog'></l-header>
                        </v-col>
                        <v-col cols='6' v-for='blog in body' :key='blog.id'>
                            <l-blog :blog='blog'></l-blog>
                        </v-col>
                    </v-row>

                    <v-row no-gutters align="center" justify="end">
                        <v-col cols='1'>
                            {{this.pagination.page}} {{$t('of')}} {{this.pages}}
                        </v-col>
                        <v-col cols='1'>
                            <v-pagination
                                v-model="pagination.page"
                                :length="pages"
                                :total-visible="0"
                                @next="getBlogs">
                            </v-pagination>
                        </v-col>
                    </v-row>

                </v-col>
            </v-row>
        </template>
    </v-container>
</template>

<script>
import Observer from '@/shared/Observers/EdgeObserver'
import ScrollToTop from '@/shared/Observers/ScrollToTop'
import Header from '@/components/blogs/header'
import Blog from '@/components/blogs/blog'
import Social from '@/shared/Media/Social'

export default {
    head() {
        return {
            title: `Layhgobuilder | Blog`
        }
    },
    data() {
        return {
            loading: false,
            query: null,
            offsetTop:0,
            headCount: 2,
            today: new Date().getTime(),
            search: "",
            searching: "",
            pagination: {
                page: 1,
                itemPerPage: 8
            },
            filterSearch: '',
            filter: 'all',
            filters: [
                'all',
                'news',
                'update',
                'showcase',
                'changelog',
                'new_assets'
            ]
        }
    },
    created() {
        this.getBlogStats();
    },
    components: {
        'l-observer': Observer,
        'l-scrolltotop': ScrollToTop,
        'l-header': Header,
        'l-blog': Blog,
        'l-social': Social
    },
    watch: {
        filter (val) {
            if (this.filter === null) { return; }
            this.pagination.page = 1;
        },
        search: function(ns) {
            if (ns === undefined || (ns !== null && ns.length <= 0)) {
                this.searching = this.search;
            }
        }
    },
    computed: {
        stats() {
            let stats = this.$store.getters['blogs/stats'];
            return stats;
        },
        blogs() {
            let blogs = this.$store.getters['blogs/blogs'];
            blogs = Object.values(blogs).filter(blog => { 
                
                if (this.searching) {
                    let searchValue = this.searching.toLowerCase();
                    let blogName = blog.title.toLowerCase();

                    let tagged = blog.tags?.some(tag => tag.text.toLowerCase().includes(searchValue));
                    return blogName.includes(searchValue) || tagged;
                }

                if (this.filter !== 'all') {
                    let tagged = blog.tags?.some(tag => tag.text.toLowerCase().includes(this.filter));
                    return tagged;
                }

                return blog.time <= this.today && blog.active;
            });
            return blogs.sort(function(a,b) { return b.published.seconds - a.published.seconds });
        },
        header() {
            return this.blogs.slice(0,this.headCount);
        },
        body() {
            return this.blogs.slice(this.headCount, this.blogs.length);
        },
        pages() {
            let length = this.total;
            if ((this.searching || this.filter !== 'all') && this.blogs) {
                length = this.blogs.length;
            }
            let size = Math.ceil(length / this.pagination.itemPerPage);    
            
            return (size <= 0 || isNaN(size)) ? 1 : size;
        },
        total() {
            return (this.stats && this.stats.published) ? this.stats.published : 1; // exclude current user projects from the total
        },
        start() {
            return (this.pagination.page - 1) * this.pagination.itemPerPage;
        },
        end() {
            return this.start + this.pagination.itemPerPage;
        },
    },
    methods: {
        getBlogStats() {
            if (this.stats.total !== undefined) { 
                this.getBlogs();
                return; 
            }
            
            this.$store.dispatch('blogs/getStats')
            .then(res => {
                this.getBlogs();
            }, err => {
                console.log(err);
            });
        },
        hasPermission(perms) {
            return this.$store.getters['auth/hasPermission']([ "manage_blog", ...perms ]);
        },
        getBlogs() {
            if (this.blogs.length >= this.end) { return; }

            this.$store.dispatch('blogs/getBlogs', { query: this.query, limit: this.pagination.itemPerPage })
            .then( (res) => {
                if (res === undefined) return;
                this.query = res.query;
            }, error => {
                console.log(error);
            });
        },
        searchBlog(e) {
            this.searching = this.search;
        },
        onScroll() {
            this.offsetTop = window.scrollY;
        }
    }
}
</script>

<style lang="scss">
.v-select {
  height:40px;
}
</style>