<template>
    <v-container>
        <input ref='upload' type="file" accept=".lb" @change='readImportFile' hidden>
        
        <v-row class='mt-2'>
            <v-col>
                <v-row align="center" class="mb-2">
                    <v-col cols='5'>
                        <span class='text-h2'>{{$t('my_projects')}}</span>
                    </v-col>

                    <v-spacer></v-spacer>

                    <v-col class="d-flex" cols='5' md='4' lg='3' xl='2'>
                        <v-select
                            dense
                            outlined
                            :label="$t('filter')"
                            v-model="filter"
                            :items='filters'>
                            <template v-slot:selection='{item}'>
                                {{$t(item)}}
                            </template>
                            <template v-slot:item='{item}'>
                                {{$t(item)}}
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
                            append-icon="mdi-magnify"
                            v-model='search'
                            @click:append="searchProject"
                            @click:clear="searching = ''">
                        </v-text-field>
                    </v-col>

                    <v-col cols='auto' v-if='hasPermission(["import_project"])'>
                        <v-tooltip right>
                            <template v-slot:activator='{ on }'>
                                <v-btn outlined v-on='on' @click="$refs.upload.click()">
                                    Import
                                    <v-icon>mdi-import</v-icon>
                                </v-btn>
                            </template>
                            {{$t('import')}}
                        </v-tooltip>
                    </v-col>
                </v-row>

                <v-divider></v-divider>

                <v-row class='pa-2 mt-2'>
                    <v-col
                        cols='12' lg='6' xl='3'
                        class='d-flex child-flex'
                        v-for='(project,n) in projects.slice(start, end)'
                        :key='n'>
                        <l-project :project='project' route="projects-projectName"></l-project>
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
                            @next="getSharedProjects">
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
            title: `Layhgobuilder | My Projects`,
        }
    },
    data() {
        return {
            tabs: 0,
            importName: null,
            importing: false,
            search: "",
            searching: "",
            query: null,
            pagination: {
                page: 1,
                itemPerPage: 8
            },
            filter: 'all',
            filters: [
                'all',
                'personal',
                'shared',
            ]
        }
    },
    created() {
        if (this.user !== undefined) {
            this.getSharedProjects();
        }

        this.$bus.$on('onAuthChanged', id => {
            if (id !== undefined) {
                this.getSharedProjects();
            }
		})
    },
    components: {
        'l-project': Project,
    },
    watch: {
        search: function(ns) {
            if (ns === undefined || (ns !== null && ns.length <= 0)) {
                this.searching = this.search;
            }
        },
        filter: function(nf) {
            if (this.filter.value === null) { return; }
            this.pagination.page = 1;
        }
    },
    computed: {
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        projects() {
            let projects = Object.values(this.$store.getters['projects/projects']);

            let stats = ['VWjXPWccqBCRu9SXQjQd', 'j46wV85HuRVGNDoC5NiK'];
            projects = projects.filter( project => {
                if (project === null) return false;

                let shared = project.shared !== undefined ? project.shared[this.user.id] : {};
                let filter = project.creatorId === this.user.id || stats.includes(shared);
                switch (this.filter) {
                    case 'personal':
                        filter = project.creatorId === this.user.id;
                        break;
                    case 'shared':
                        filter = stats.includes(shared);
                        break;
                }                
                return filter;
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
            
            return projects.sort( (a,b) => (a, b) => (a.projectName > b.projectName) ? 1 : -1);
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
            let stats = this.user['--stats--'];
            let projects = stats?.projects;
            let shared = stats?.shared ? stats.shared : 0;

            console.log(projects, shared);
            return projects + shared;
        },
        start() {
            return (this.pagination.page - 1) * this.pagination.itemPerPage;
        },
        end() {
            return this.start + this.pagination.itemPerPage;
        }
    },
    methods: {
        hasPermission(permissions) {
            return this.$store.getters['auth/hasPermission']([ "manage_projects", ...permissions ]);
        },
        readImportFile(event) {
            this.importing = true;
            this.importName = event.target.files[0].name;
            this.importName = this.importName.split('.').slice(0, -1).join('.');

            var reader = new FileReader();
            reader.onload = this.importProject;
            reader.readAsText(event.target.files[0]);
        },
        searchProject(e) {
            this.searching = this.search;
            this.$store.dispatch('projects/getProjectByName', this.searching)
            .then( res => {
                
            }, error => {
                
            });
        },
        getSharedProjects() {
            if (this.projects.length >= this.end || this.projects.length >= this.total) { return; }

            this.$store.dispatch('projects/getSharedProjectsByUser', { limit: this.pagination.itemPerPage })
            .then( res => {
                if (res === undefined) return;
                this.query = res.query;
            }, error => {
                console.log(error);
            });
        },
        async importProject(event) {  
            if (this.importName === null) {
                this.importName = "New Project";
            }
            let project = JSON.parse(event.target.result);
            console.log(project);
            await this.$store.dispatch('projects/importProject', { 
                creatorId: this.user.id,
                creatorName: this.user.username,
                projectName: this.importName, 
                json: project 
            });

            this.importName = null;
            this.importing = false;
            console.log("project imported");
        }
    }
}
</script>

<style lang="scss">
.v-select {
  height:40px;
}
</style>