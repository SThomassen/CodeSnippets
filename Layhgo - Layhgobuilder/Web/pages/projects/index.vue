<template>
    <v-container>
        <v-row class='mt-2'>
            <v-col>
                <v-row align="center" class='mb-2'>
                    <v-col cols='auto'>
                        <div class='text-h2'>{{ $t("projects") }}</div>
                    </v-col>

                    <v-spacer></v-spacer>

                    <v-col class="d-flex" cols='5' md='4' lg='3' xl='2'>
                        <v-select
                            dense
                            outlined
                            :label='$t("filter")'
                            class='pr-2 pl-2'
                            v-model="filter"
                            :items='filters'>
                            <template v-slot:selection='{item}'>
                                {{ $t(item) }}
                            </template>
                            <template v-slot:item='{item}'>
                                {{ $t(item) }}
                            </template>
                        </v-select>
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
                            @click:append="searchProject"
                            @click:clear="searching = ''">
                        </v-text-field>
                    </v-col>
                </v-row>

                <v-divider></v-divider>

                <v-row class='pa-2 mt-2'>
                    <v-col
                        cols='12' md='6' lg='4' xl='3'
                        class='d-flex child-flex'
                        v-for='(project,n) in projects.slice(start,end)'
                        :key='n'>
                        <l-project :project='project' route='projects-projectName'></l-project>
                    </v-col> 
                </v-row>

                <v-row no-gutters align="center" justify="end">
                    <v-col cols='1'>
                         {{this.start}} - {{this.end}} {{ $t("of") }} {{this.total}}
                    </v-col>
                    <v-col cols='1'>
                        <v-pagination
                            v-model="pagination.page"
                            :length="pages"
                            :total-visible="0"
                            @next="getProjects">
                        </v-pagination>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
import Project from '@/components/projects/project'

export default {
    head() {
        return {
            title: `Layhgobuilder | Projects`,
        }
    },
    data() {
        return {
            search: "",
            searching: "",
            loading: false,
            pagination: {
                page: 1,
                itemPerPage: 20
            },
            filter: 'all',
            filters: [
                "all",
                // "event",
                "showcase",
                "community",
            ]
        }
    },
    mounted() {
        this.getProjectStats();  
        
        window.addEventListener('unload', this.unloadData)

        let page = parseInt(localStorage.builders);
        this.pagination.page = !isNaN(page) ? page : 1;
    },
    beforeDestroy() {
        window.removeEventListener('unload', this.unloadData)
    },
    components: {
        'l-project': Project
    },
    watch: {
        search: function(ns) {
            if (ns === undefined || (ns !== null && ns.length <= 0)) {
                this.searching = this.search;
            }
        },
        filter: function(nf) {
            if (this.filter === null) { return; }
            this.pagination.page = 1;
            this.getProjects();
        },
        pagination: {
            handler () {
                localStorage.setItem('projects', this.pagination.page);
            },
            deep: true
        }
    },
    computed: {
        stats() {
            let stats = this.$store.getters['projects/stats'];
            return stats;
        },
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        projects() {
            let projects = Object.values(this.$store.getters['projects/projects']);
            projects = projects.filter( project => { 
                if (project.invoked ||
                    !project.published ||
                    project.published === undefined) { 
                    return false;
                }

                if (this.filter === 'event') {
                    return false;
                }

                if (this.filter === 'community' && project.showcase) {
                    return false;
                }
                if (this.filter === 'showcase' && !project.showcase) {
                    return false;
                }
                return true;
            });

            let search = this.searching.trim().toLowerCase();
            if (search) {
                projects = projects.filter(item => {
                    return Object.values(item)
                        .join(",")
                        .toLowerCase()
                        .includes(search);
                });
            }

            return projects.sort((a, b) => (a.projectName > b.projectName) ? 1 : -1);
        },
        pages() {
            let length = this.total;
            if ((this.searching || this.filter !== 'all') && this.projects) {
                length = this.projects.length;
            }
            let size = Math.ceil(length / this.pagination.itemPerPage);    
            
            return (size <= 0 || isNaN(size)) ? 1 : size;
        },
        total() {
            if (this.stats.published === undefined) { 
                return 1;
            }
            if (this.filter === 'showcase') {
                return this.stats.showcase;
            }
            if (this.filter === 'community') {
                return this.stats.published - this.stats.showcase;
            }

            return this.stats.published; // exclude current user projects from the total
        },
        start() {
            return (this.pagination.page - 1) * this.pagination.itemPerPage;
        },
        end() {
            return this.start + this.pagination.itemPerPage;
        },
    },
    methods: {
        unloadData() {
            localStorage.setItem('projects', 1);
        },
        getProjectStats() {
            if (this.stats.total !== undefined) { 
                this.getProjects();
                return; 
            }
            
            this.$store.dispatch('projects/getStats').then(() => {
                this.getProjects();
            }, err => {
                console.log(err);
            });
        },
        getProjects() {
            if (this.projects.length >= this.end) { return; }

            this.loading = true;
            this.$store.dispatch('projects/getCommunityProjects', { filter: this.filter, limit: this.pagination.itemPerPage })
            .then( (res) => {
                this.loading = false;
            }, error => {
                console.log(error);
                this.loading = false;
            });
        },
        searchProject(e) {
            this.searching = this.search;
            this.$store.dispatch('projects/getProjectByName', this.searching.toLowerCase().trim())
            .then( res => {
                
            }, error => {
                
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