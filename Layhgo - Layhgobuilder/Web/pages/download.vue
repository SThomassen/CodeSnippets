<template>
    <v-container>
        <template>
            <l-upload ref='upload'></l-upload>
            <l-scrolltotop :y='offsetTop'></l-scrolltotop>
            
            <v-layout>
                <v-row v-scroll='onScroll' class='mt-2'>
                    <v-col>
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
                        
                        <template v-if='hasPermission(["access_standalone"])'>
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
                        <template v-else>
                            <v-row class='pl-2 pr-2 mt-4' justify="center">
                                <v-col>
                                    <p>You need register your account to gain access to the standalone.</p>
                                </v-col>
                            </v-row>
                            <v-row no-gutters class='pl-2 pr-2'>
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
                            </v-row>
                        </template>

                        <v-row>
                            <v-col>
                                <span class='title'>{{$t('release_notes')}}</span>
                            </v-col>
                            <v-spacer></v-spacer>
                            <v-col cols='auto'>
                                <v-btn-toggle
                                    group
                                    mandatory
                                    v-model="filter">
                                    <v-btn small outlined value="standalone">Standalone</v-btn>
                                    <v-btn small outlined value="fieldlab">Fieldlab</v-btn>
                                    <v-btn small outlined value="web">Web</v-btn>
                                </v-btn-toggle>
                            </v-col>
                        </v-row>

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
    </v-container>
</template>

<script>
import Observer from '@/shared/Observers/EdgeObserver'
import ScrollToTop from '@/shared/Observers/ScrollToTop'
import Timestamp from '@/shared/Input/Timestamp'
import Upload from '@/components/download/upload'
import Release from '@/components/download/release'

export default {
    head() {
        return {
            title: `Layhgobuilder | Download`,
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
    middleware({ app, store, redirect }) {
        // If the user is not authenticated
        if (!store.getters['auth/user']) {
            return redirect(app.localePath('/'))
        }
    },
    components: {
        'l-observer': Observer,
        'l-scrolltotop': ScrollToTop,
        'l-timestamp': Timestamp,
        'l-upload': Upload,
        'l-release': Release
    },
    computed: {
        releases() {
            let releases = this.$store.getters['releases/versions'];
            return releases ? releases : {};
        },
        latest() {
            let latest = Object.values(this.releases).find(release => release.type === 'official' && release.platform === 'standalone');
            return latest ? latest : {};
        },
        beta() {
            let beta = Object.values(this.releases).find(release => release.type === 'beta' && release.platform === 'standalone');
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
                let license = await this.$store.dispatch('licenses/submitLicense', { key: this.licenseKey, userId: "", username: "" });
            } catch(err) {
                console.log(err);
            }
        }
    }
}
</script>
