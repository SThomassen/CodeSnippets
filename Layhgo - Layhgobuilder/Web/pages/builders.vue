<template>
    <v-container>
            <v-row class='mt-2'>
                <v-col>
                    <template v-if='authenticated'>
                        <v-row align="center" class='mb-2'>
                            <v-col cols='6'>
                                <span class='text-h2'>{{ $t('builders') }}</span>
                            </v-col>
                            
                            <v-spacer></v-spacer>

                            <v-col class='d-flex' cols='2'>
                                <v-select
                                    dense
                                    outlined
                                    return-object
                                    :label="$t('filter')"
                                    class='pr-2 pl-2'
                                    item-text="text"
                                    item-value="value"
                                    v-model="filter"
                                    :items='filters'>
                                    <template v-slot:selection='{ item }'>
                                        {{ $t(item.text) }}
                                    </template>
                                    <template v-slot:item='{ item }'>
                                        {{ $t(item.text) }}
                                    </template>
                                </v-select>
                            </v-col>

                            <v-col class='d-flex' cols='2'>
                                <v-text-field
                                    dense
                                    clearable
                                    outlined
                                    hide-details
                                    :label='$t("search")'
                                    class='pr-2 pl-2'
                                    append-icon="mdi-magnify"
                                    v-model='search'
                                    @click:append="searchUser"
                                    @click:clear="searching = ''">
                                </v-text-field>
                            </v-col>
                        </v-row>

                        <v-divider></v-divider>

                        <v-row class='mt-2'>
                            <v-col cols='12' class='pa-2'>
                                <v-list dense>
                                    <l-builder v-for='(builder,i) in builders.slice(start, end)' :key='i' :builder='builder'></l-builder>
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

                        <v-row class='mt-1' align="center" justify="end">
                            <v-spacer></v-spacer>
                            <v-col cols='auto'>
                                {{this.start}} - {{this.end}} {{ $t('of')}} {{this.total}}
                            </v-col>
                            <v-col cols='1'>
                                <v-pagination
                                    v-model="pagination.page"
                                    :length="pages"
                                    :total-visible="0"
                                    @next="getBuilders">
                                </v-pagination>
                            </v-col>
                        </v-row>
                    </template>
                    <template v-else>
                        <span class='text-h2'>{{ $t('available_builder') }}.</span>
                        <p class='title'>{{ $t('register_builder') }}.</p>
                    </template>
                </v-col>
            </v-row>
    </v-container>
</template>

<script>
import Builder from '@/components/Builders/Builder'


export default {
    head() {
        return {
            title: `Layhgobuilder | Builders`,
        }
    },
    data() {
        return {
            search: "",
            searching: "",
            loading: false,
            pagination: {
                page: 1,
                itemsPerPage: 20
            },
            filter: { text: 'all', value: 'all' },
            filters: [
                { text: 'all', value: 'all' },
                { text: 'moderator', value: 'XMcqbry1K14Czo1Ndo2O' },
                { text: 'administrator', value: 'FmJKOcHOsVxmU2OY7ekQ' },
                // { text: 'organizer', values: 'organizer' }
            ],
        }
    },
    mounted() {
        window.addEventListener('unload', this.unloadData)

        let page = parseInt(localStorage.builders);
        this.pagination.page = !isNaN(page) ? page : 1;
        this.$nextTick( () => {
            if (this.authenticated) {
                this.getBuildersStats();
            }
        });

        this.$bus.$on('onAuthChanged', id => {
            if (id !== undefined) {
                this.getBuildersStats();
            }
		})
    },
    beforeDestroy() {
        window.removeEventListener('unload', this.unloadData)
    },
    components: {
        'l-builder': Builder
    },
    watch: {
        search: function(ns) {
            if (ns === undefined || (ns !== null && ns.length <= 0)) {
                this.searching = this.search;
            }
        },
        filter: function(nf) {
            if (this.filter.value === 'all') { return; }
            this.pagination.page = 1;
            this.getBuilders();
            // this.getBuildersByRole();
        },
        pagination: {
            handler () {
                localStorage.setItem('builders', this.pagination.page);
            },
            deep: true
        }
    },
    computed: {
        stats() {
            return this.$store.getters['auth/stats'];
        },
        builders() {
            let builders = Object.values(this.$store.getters['auth/builders']);
            builders = builders.filter( builder => { 
                if (builder.id === this.user.id) {
                    return false;
                }

                if (this.filter.value !== 'all' && !(this.filter.value in builder.claims)) {
                    return false;
                }

                return true;
            });

            let search = this.searching.trim().toLowerCase();
            if (search) {
                builders = builders.filter(item => {
                    return Object.values(item)
                        .join(",")
                        .toLowerCase()
                        .includes(search);
                });
            }

            return builders.sort((a, b) => (a.username > b.username) ? 1 : -1);
        },
        user() {
            let user = this.$store.getters['auth/user'];
            return (user) ? user : {};
        },
        authenticated() {
            let user = this.$store.getters['auth/user'];
            return user !== null && user !== undefined;
        },
        pages() {
            let length = (this.searching && this.builders) ? this.builders.length : this.total;
            if ((this.searching || this.filter.value !== 'all') && this.builders) {
                length = this.builders.length;
            }
            let size = Math.ceil(length / this.pagination.itemsPerPage);    
            return (size <= 0 || isNaN(size)) ? 1 : size;
        },
        private() {
            return this.stats.private;
        },
        total() {
            return (this.stats && this.stats.total) ? this.stats.total - 1 - this.private : 1; // needs to be -1 to exclude current user
        },
        start() {
            return (this.pagination.page - 1) * this.pagination.itemsPerPage;
        },
        end() {
            return this.start + this.pagination.itemsPerPage;
        },
    },
    methods: {
        unloadData() {
            localStorage.setItem('builders', 1);
        },
        getBuildersStats() {
            if (this.stats.total !== undefined) { 
                this.getBuilders();
                return; 
            }

            this.$store.dispatch('auth/getStats').then(() => {
                this.getBuilders();
            }, err => {
                console.log(err);
            });
        },
        getBuilders() {
            if (this.builders.length >= this.end) { return; }

            this.loading = true;
            this.$store.dispatch('auth/getBuilders', { limit: this.pagination.itemsPerPage + 1, role: this.filter.value }) // cannot hurt to get 1 more builder, just in case it is the current user.
            .then( () => {
                this.loading = false;
            }, error => {
                console.log(error);
                this.loading = false;
            });
        },
        searchUser(e) {
            this.searching = this.search;
            this.$store.dispatch('auth/getBuilderByName', this.searching.trim().toLowerCase())
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