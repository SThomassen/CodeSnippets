<template>
    <v-container>

        <v-row class='mt-2'>
            <v-col>
                <v-row class='mb-2' align='center'>
                    <v-col cols='5'>
                        <span class='text-h2'>{{ $t('reports.title') }}</span>
                    </v-col>

                    <v-spacer></v-spacer>

                    <!-- <v-btn-toggle
                        v-model="viewMode"
                        mandatory>
                        <v-btn>
                            <v-icon>mdi-format-align-justify</v-icon>
                        </v-btn>
                        <v-btn>
                            <v-icon>mdi-view-column</v-icon>
                        </v-btn>
                    </v-btn-toggle> -->
                </v-row>

                <v-divider></v-divider>

                <v-row>
                    <v-col>
                        <v-row class='pa-2 mt-2'>
                            <v-col cols='12'>
                                <v-list>
                                    <v-card 
                                        outlined 
                                        class='mb-2' 
                                        v-for='report in reports.slice(this.start, this.end)' 
                                        :to='localePath({ name: "reports-reportName", params: { reportName: report.title.replace(/[^a-zA-Z0-9]/g,"_") }, query: { id: report.id } })'
                                        :key='report.id'>
                                        <l-report :report='report'></l-report>
                                    </v-card>
                                </v-list>
                            </v-col>
                        </v-row>

                        <template v-if='loading'>
                            <v-container>
                                <v-row align="center" justify="center">
                                    <v-col cols='auto'>
                                        <v-progress-circular
                                            indeterminate
                                            :size='40'>    
                                        </v-progress-circular>
                                    </v-col>
                                </v-row>
                            </v-container>
                        </template>

                        <v-row no-gutters align="center" justify="end">
                            <v-col cols='1'>
                                {{this.start}} - {{this.end}} {{$t('of')}} {{this.total}}
                            </v-col>
                            <v-col cols='1'>
                                <v-pagination
                                    v-model="pagination.page"
                                    :length="pages"
                                    :total-visible="0"
                                    @next="getReports">
                                </v-pagination>
                            </v-col>
                        </v-row>
                    </v-col>

                    <v-col md='2'>
                        <v-row align='center' class='pa-2 mt-2'>
                            <v-col cols='12'>
                                <v-tooltip right>
                                    <template v-slot:activator='{ on }'>
                                        <v-btn color="primary" outlined block v-on='on' :to='localePath({ name: "reports-create" })'>
                                            {{ $t('reports.create') }}
                                            <v-icon>mdi-plus</v-icon>
                                        </v-btn>
                                    </template>
                                    {{ $t('reports.create') }}
                                </v-tooltip>
                            </v-col>

                            <v-col cols='12'>
                                <v-text-field
                                    dense
                                    outlined
                                    clearable
                                    hide-details
                                    :label='$t("search")'
                                    append-icon="mdi-magnify"
                                    v-model='search'
                                    @click:append="searchReport"
                                    @click:clear="searching = ''">
                                </v-text-field>
                            </v-col>

                            <v-col cols='12'>
                                <v-combobox
                                    v-model="filterPlatform"
                                    dense
                                    outlined
                                    persistent-hint
                                    :label="$t('reports.product.title')"
                                    :items="product"
                                    :search-input.sync="filterSearch">
                                    <template v-slot:selection='{item}'>
                                        {{ $t(item.text) }}
                                    </template>
                                    <template v-slot:item='{item}'>
                                        {{ $t(item.text) }}
                                    </template>
                                </v-combobox>
                            </v-col>

                            <v-col cols='12'>
                                <v-combobox
                                    v-model="filterType"
                                    dense
                                    outlined
                                    persistent-hint
                                    :label="$t('reports.types.title')"
                                    :items="types"
                                    :search-input.sync="filterSearch">
                                    <template v-slot:selection='{item}'>
                                        {{ $t(item.text) }}
                                    </template>
                                    <template v-slot:item='{item}'>
                                        {{ $t(item.text) }}
                                    </template>
                                </v-combobox>
                            </v-col>

                            <v-col cols='12'>
                                <v-combobox
                                    v-model="filterState"
                                    dense
                                    outlined
                                    persistent-hint
                                    item-value="id"
                                    item-text='text'
                                    :label="$t('reports.states.title')"
                                    :items="states"
                                    :search-input.sync="filterSearch">
                                    <template v-slot:selection='{item}'>
                                        {{ $t(item.text) }}
                                    </template>
                                    <template v-slot:item='{item}'>
                                        {{ $t(item.text) }}
                                    </template>
                                </v-combobox>
                            </v-col>

                            <v-col cols='12'>
                                <v-combobox
                                    v-model="filterVersion"
                                    dense
                                    outlined
                                    persistent-hint
                                    :label="$t('reports.version.title')"
                                    :items="versions"
                                    :search-input.sync="filterSearch">
                                    <template v-slot:selection='{item}'>
                                        {{ item.text }}
                                    </template>
                                    <template v-slot:item='{item}'>
                                        {{ item.text }}
                                    </template>
                                </v-combobox>
                            </v-col>
                        </v-row>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
import Avatar from '@/shared/Cards/Avatar'
import Report from '@/components/reports/report'
import info from '@/config/report'

export default {
    head() {
        return {
            title: `Layhgobuilder | Reports`,
        }
    },
    data() {
        return {
            loading: false,
            search: "",
            searching: "",
            pagination: {
                page: 1,
                itemPerPage: 20
            },
            viewMode: 0,
            filterSearch: '',
            filterState: { id: 'all', text: 'all' },
            filterType: { id: 'all', text: 'all' },
            filterPlatform: { id: 'all', text: 'all' },
            filterVersion: { id: 'all', text: this.$t('all') },
            today: new Date().getTime(),
        }
    },
    created() {
        this.getReportStats();
    },
    components: {
        'l-avatar': Avatar,
        'l-report': Report
    },
    watch: {
        filterState (val) {
            if (this.filterState.id === 'all') { return; }
            this.pagination.page = 1;
            this.getReports();
        },
        search: function(ns) {
            if (ns === undefined || (ns !== null && ns.length <= 0)) {
                this.searching = this.search;
            }
        }
    },
    computed: {
        stats() {
            return this.$store.getters['reports/stats'];
        },
        reports() {

            // var reports = {};
            // for (let i = 0; i < 100; i++) {
            //     reports[i] = {
            //         id: i,
            //         title: `test_${i}`,
            //         created: new Date(),
            //         updated: new Date(),
            //         type: `${i % 3 == 0 ? "feature" : "bug"}`,
            //         state: "open"
            //     }
            // }

            let reports = this.$store.getters['reports/reports'];
            reports = Object.values(reports).filter(report => { 
                
                if (this.searching) {
                    let searchValue = this.searching.toLowerCase();
                    let reportName = report.title.toLowerCase();

                    let tagged = report.tags?.some(tag => tag.text.toLowerCase().includes(searchValue));
                    return reportName.includes(searchValue) || tagged;
                }
                
                let filters = {};
                filters.state = this.filterState.id === report.state || this.filterState.id === 'all';
                filters.type = this.filterType.id === report.type || this.filterType.id === 'all';
                filters.product = this.filterPlatform.id === report.product || this.filterPlatform.id === 'all';
                filters.version = this.filterVersion.id === report.version || this.filterVersion.id === 'all';
                
                return Object.values(filters).every( filter => filter);
            });
            return reports.sort(function(a,b) { return b.updated.seconds - a.updated.seconds });
        },
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        pages() {
            let length = this.total;
            // if ((this.searching) && this.reports) {
            //     length = this.reports.length;
            // }
            let size = Math.ceil(length / this.pagination.itemPerPage);    
            
            return (size <= 0 || isNaN(size)) ? 1 : size;
        },
        total() {
            return (this.stats && this.stats.total) ? this.stats.total : 1;
        },
        start() {
            return (this.pagination.page - 1) * this.pagination.itemPerPage;
        },
        end() {
            return Math.min(this.start + this.pagination.itemPerPage, this.total);
        },
        types() {
            let types = [ { id: "all", text: "all", color: "primary" }, ...info.types ];
            return types;
        },
        states() {
            let states = [ { id: "all", text: "all", color: "primary" }, ...info.states ];
            return states;
        },
        product() {
            let product = [ { id: "all", text: "all", color: "primary" }, ...info.product ];
            return product;
        },
        versions() {
            let releases = Object.values(this.$store.getters['releases/versions']).map(release => { return { id: release.id, text: release.id } });
            let versions = [ { id: "all", text: this.$t("all"), color: "primary" }, ...releases ];
            return versions
        }
    },
    methods: {
        hasPermission(perms) {
            return this.$store.getters['auth/hasPermission']([ "manage_blog", ...perms ]);
        },
        searchReport(e) {
            this.searching = this.search;
        },
        getReportStats() {
            if (this.stats.total !== undefined) { 
                this.getReports();
                return; 
            }
            
            this.$store.dispatch('reports/getStats')
            .then(res => {
                this.getReports();
            }, err => {
                console.log(err);
            });
        },
        getReports() {
            if (this.reports.length >= this.end) { return; }

            console.log(this.filterState.id);
            this.loading = true;
            this.$store.dispatch('reports/getReports', { filter: this.filterState.id, limit: this.pagination.itemPerPage })
            .then( () => {
                this.loading = false;
            }, error => {
                console.log(error);
                this.loading = false;
            });
        },
    }
}
</script>

<style lang="scss">
.v-select {
  height:40px;
}
</style>