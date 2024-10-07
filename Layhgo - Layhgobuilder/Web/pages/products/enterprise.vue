<template>
    <v-container>
        <l-purchase></l-purchase>
        <v-row class='mt-2'>
            <v-col>
                <v-row>
                    <v-col>
                        <v-parallax src='images/layhgobuilder_enterprise.png'>
                            <v-row align="end">
                                <v-col cols='12'>
                                    <v-sheet dark color="rgba(0, 0, 0, .4)">
                                        <v-row class='pa-2'>
                                            <v-col cols='12'>
                                                <span class='text-h2'>Layhgobuilder Enterprise</span>
                                            </v-col>

                                            <v-col>
                                                <v-btn outlined v-if='hasPermission(["access_enterprise"])' @click='download(latest.destination, getOS.key)'>
                                                    {{$t('products.download_for', { name: getOS.value}) }}
                                                </v-btn>
                                                <v-btn outlined v-else @click='$bus.$emit("onOpenEnterpriseLicense")'>
                                                    {{$t('products.get_access')}}
                                                </v-btn>
                                            </v-col>
                                        </v-row>
                                    </v-sheet>
                                </v-col>
                            </v-row>
                        </v-parallax>
                    </v-col>
                </v-row>

                <!-- <v-row>
                    <v-col>
                        <span v-html="lorum"></span>
                    </v-col>
                    <v-col>
                        <span v-html="lorum"></span>
                    </v-col>
                    <v-col>
                        <span v-html="lorum"></span>
                    </v-col>
                </v-row>

                <v-row>
                    <v-col>
                        <span v-html="lorum"></span>
                    </v-col>
                </v-row> -->

                <template>
                    <l-upload ref='upload'></l-upload>
                    <l-scrolltotop :y='offsetTop'></l-scrolltotop>
                    
                    <v-layout>
                        <v-row v-scroll='onScroll' class='mt-2'>
                            <v-col cols='12'>
                                <v-row>
                                    <v-col cols='6'>
                                        <span class='title'>{{$t('download')}}</span>
                                        
                                    </v-col>
                                    <v-spacer></v-spacer>
                                    <v-col cols='auto'>
                                        <v-tooltip right v-if='hasPermission(["upload_product"])'>
                                            <template v-slot:activator='{ on }'>
                                                <v-btn small icon v-on='on' @click.prevent='$refs.upload.open()'>
                                                    <v-icon>mdi-cloud-upload-outline</v-icon>
                                                </v-btn>
                                            </template>
                                            {{ $t('upload') }}
                                        </v-tooltip>
                                        <!-- <l-social></l-social> -->
                                    </v-col>
                                </v-row>

                                <v-divider></v-divider>
                                
                                <template v-if='hasPermission(["access_enterprise"])'>
                                    <v-row class='pa-2'>
                                        <v-col class='text-h4' cols='6'>
                                            Windows
                                        </v-col>
                                        <v-col class='text-h4' cols='6'>
                                            Mac
                                        </v-col>
                                    </v-row>

                                    <!-- official -->
                                    
                                    <v-row no-gutters class='pa-2'>
                                        <v-col class='pr-2' sm='6'>
                                            <v-card color="secondary" dark>
                                                <v-card-title class="text-h5">
                                                    Layhgobuilder {{latest.id}}
                                                    <v-spacer></v-spacer>
                                                    <v-chip>{{latest.type}}</v-chip>
                                                </v-card-title>

                                                <v-card-subtitle>
                                                    <l-timestamp :date='latest.date' format="short"></l-timestamp><!--MMMM Do YYYY-->
                                                </v-card-subtitle>

                                                <v-card-actions>
                                                    <v-btn text @click='download(latest.destination, "win")'  >
                                                        {{$t('download')}}
                                                    </v-btn>
                                                </v-card-actions>
                                            </v-card>
                                        </v-col>

                                        <v-col class='pl-2' sm='6'>
                                            <v-card color="secondary" dark>
                                                <v-card-title class="text-h5">
                                                    Layhgobuilder {{latest.id}}
                                                    <v-spacer></v-spacer>
                                                    <v-chip>{{latest.type}}</v-chip>
                                                </v-card-title>

                                                <v-card-subtitle>
                                                    <l-timestamp :date='latest.date' format="short"></l-timestamp><!--MMMM Do YYYY-->
                                                </v-card-subtitle>

                                                <v-card-actions>
                                                <v-btn text @click='download(latest.destination, "mac")'>
                                                    {{$t('download')}}
                                                </v-btn>
                                                </v-card-actions>
                                            </v-card>
                                        </v-col>
                                    </v-row>

                                    <v-row no-gutters class='pa-2' v-if='beta.id !== undefined'>
                                        <v-col class='pr-2' sm='6'>
                                            <v-card>
                                                <v-list-item dense two-line>
                                                    <v-list-item-content>
                                                        <v-list-item-title>
                                                            Layhgobuilder {{beta.id}}
                                                            <v-chip x-small>{{beta.type}}</v-chip>
                                                        </v-list-item-title>

                                                        <v-list-item-subtitle>
                                                            <l-timestamp :date='beta.date' format="short"></l-timestamp><!--MMMM Do YYYY-->
                                                        </v-list-item-subtitle>
                                                    </v-list-item-content>
                                                    <v-list-item-action-text>
                                                    <v-btn x-small text @click='download(beta.destination, "win")'>
                                                        {{$t('download')}}
                                                    </v-btn>
                                                    </v-list-item-action-text>
                                                </v-list-item>
                                            </v-card>
                                        </v-col>
                                        <v-col class='pl-2' sm='6'>
                                            <v-card>
                                                <v-list-item dense two-line>
                                                    <v-list-item-content>
                                                        <v-list-item-title>
                                                            Layhgobuilder {{beta.id}}
                                                            <v-chip x-small>{{beta.type}}</v-chip>
                                                        </v-list-item-title>
                                                        
                                                        <v-list-item-subtitle>
                                                            <l-timestamp :date='beta.date' format="short"></l-timestamp><!--MMMM Do YYYY-->
                                                        </v-list-item-subtitle>
                                                    </v-list-item-content>
                                                    <v-list-item-action-text>
                                                        <v-btn x-small text @click='download(beta.destination, "mac")'>
                                                            {{$t('download')}}
                                                        </v-btn>
                                                    </v-list-item-action-text>
                                                </v-list-item>
                                            </v-card>
                                        </v-col>
                                    </v-row>
                                </template>
                                <template v-else-if='user.id !== undefined'>
                                    <v-row class='pl-2 pr-2 mt-4' justify="center">
                                        <v-col>
                                            <p>You need register your account to gain access to the standalone.</p>
                                        </v-col>
                                    </v-row>
                                    <!-- <v-row no-gutters class='pl-2 pr-2'>
                                        <v-col class='pr-2'>
                                            <v-text-field
                                                dense
                                                outlined
                                                :label='$t("releases.license_key")'
                                                v-model='licenseKey'>
                                            </v-text-field>
                                        </v-col>
                                        <v-col>
                                            <v-btn outlined @click.prevent="submitLicense">Submit</v-btn>
                                        </v-col>
                                    </v-row> -->
                                </template>
                                <template v-else>
                                    <v-row class='ma-2'>
                                        <v-col>
                                            <v-btn 
                                                dense
                                                outlined
                                                @click.prevent='login'>
                                                {{$t('log_in')}}
                                            </v-btn>
                                        </v-col>
                                    </v-row>
                                </template>

                                

                                <v-divider></v-divider>

                                <template>
                                    <v-row>
                                        <v-col cols='12'>
                                            <v-list two-line>
                                                <template v-for="release in releases">
                                                    <l-release :key='release.id' :release='release'></l-release>
                                                </template>
                                            </v-list>
                                        </v-col>
                                    </v-row>

                                    <v-row class='mt-2' align="center" justify="end">
                                        <v-col cols='1'>
                                            {{this.pagination.page}} {{$t('of')}} {{this.pages}}
                                        </v-col>
                                        <v-col cols='1'>
                                            <v-pagination
                                                v-model="pagination.page"
                                                :length="pages"
                                                :total-visible="0"
                                                @next="getReleases">
                                            </v-pagination>
                                        </v-col>
                                    </v-row>
                                </template>

                            </v-col>
                        </v-row>
                    </v-layout>
                </template>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
import Observer from '@/shared/Observers/EdgeObserver'
import ScrollToTop from '@/shared/Observers/ScrollToTop'
import Timestamp from '@/shared/Input/Timestamp'
import Upload from '@/components/download/upload'
import Release from '@/components/download/release'
import Purchase from '@/components/products/purchase'

import debug from '@/config/debug'

export default {
    head() {
        return {
            title: `Layhgobuilder | Enterprise`,
        }
    },
    data() {
        return {
            loading: true,
            query: null,
            filter: "standalone",
            offsetTop:0,
            count: 99999,
            licenseKey: "",
            today: new Date().getTime(),
            pagination: {
                page: 1,
                itemsPerPage: 10
            },
        }
    },
    async created() {
        // this.getReleases();
        await this.getStats();
        this.getReleases();
        // this.getVersions();
    },
    components: {
        'l-observer': Observer,
        'l-scrolltotop': ScrollToTop,
        'l-timestamp': Timestamp,
        'l-upload': Upload,
        'l-release': Release,
        'l-purchase': Purchase
    },
    computed: {
        lorum() {
            return debug.lorum;
        },
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        releases() {
            let releases = this.$store.getters['releases/versions'];
            if (releases !== undefined) {
                return Object.values(releases).filter(release => release.product === 'enterprise');
            }
            return releases ? releases : {};
        },
        latest() {
            let latest = Object.values(this.releases).find(release => release.type === 'official');
            return latest ? latest : {};
        },
        beta() {
            let beta = Object.values(this.releases).find(release => release.type === 'beta');
            return beta ? beta : {};
        },
        notes() {
            let blogs = this.$store.getters['blogs/blogs'];
            
            blogs = Object.values(blogs).filter(blog => { 
                return blog.time <= this.today && 
                    blog.active && 
                    blog.releases && 
                    (blog.tags.some(e => e.text == "Changelog")) 
                }).sort(function(a,b) { return b.published.seconds - a.published.seconds; });
            return (blogs) ? blogs : [];
        },
        blogs() {
            return this.notes?.slice(this.start, this.end);
        },
        pages() {
            let length = (this.notes) ? this.notes.length : this.total;
            let size = Math.ceil(length / this.pagination.itemsPerPage);    
            return (size <= 0 || isNaN(size)) ? 1 : size;
        },
        total() {
            return (this.stats && this.stats.total) ? this.stats.total - 1 : 1; // needs to be -1 to exclude current user
        },
        start() {
            return (this.pagination.page - 1) * this.pagination.itemsPerPage;
        },
        end() {
            return this.start + this.pagination.itemsPerPage;
        },
        getOS()
        {
            var userAgent = window.navigator.userAgent,
            platform = window.navigator?.userAgentData?.platform ?? window.navigator.platform,
            macosPlatforms = ['Macintosh', 'MacIntel', 'MacPPC', 'Mac68K'],
            windowsPlatforms = ['Win32', 'Win64', 'Windows', 'WinCE'],
            iosPlatforms = ['iPhone', 'iPad', 'iPod'],
            os = null;

            if (macosPlatforms.indexOf(platform) !== -1) {
                os = { key: 'mac', value: 'Mac OS' };
            } else if (iosPlatforms.indexOf(platform) !== -1) {
                os = { key: 'ios', value: 'iOS' };
            } else if (windowsPlatforms.indexOf(platform) !== -1) {
                os = { key: 'win', value: 'Windows' };
            } else if (/Android/.test(userAgent)) {
                os = { key: 'android', value: 'Android' };
            } else if (!os && /Linux/.test(platform)) {
                os = { key: 'linux', value: 'Linux' }; 
            }

            return os;
        },
    },
    methods: {
        hasPermission(perms) {
            return this.$store.getters['auth/hasPermission']([ ...perms ]);
        },
        async getStats() {
            await this.$store.dispatch('releases/getStats');
        },
        async getVersions() {
            await this.$store.dispatch('releases/getVersions');
        },
        async getReleases() {
            this.loading = true;
            let res = await this.$store.dispatch('releases/getReleases', { query: this.query, limit: 10 });
            this.loading = false;
            
            if (res === undefined) return;
            this.query = res.query;
        },
        async download(dest, os) {
            let fileUrl = `${dest}/Layhgobuilder_${os}.zip`;
            let url = await this.$store.dispatch('storage/downloadFile', fileUrl);
            
            var a = document.createElement("a");
            a.href = url;
            a.setAttribute("download", "Layhgobuilder.zip");
            a.click();
        },
        onScroll() {
            this.offsetTop = window.scrollY;
        },
        async submitLicense(){
            try {
                let license = await this.$store.dispatch('licenses/submitLicense', { key: this.licenseKey });
            } catch(err) {
                console.log(err);
            }
        },
        login() {
            this.$bus.$emit('onOpenLogin');
        },
    }
}
</script>