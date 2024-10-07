<template>
    <v-container>
        <v-row no-gutters>
            <v-col>
                <v-data-table
                    dense
                    disable-items-per-page
                    hide-default-header
                    :headers='headers'
                    :loading='loading'
                    :items='sharedBuilders'
                    :items-per-page="10"
                    :options.sync="options"
                    :server-items-length='sharedTotal'
                    :footer-props="{
                        itemsPerPageOptions: []
                    }">
                    <template v-slot:top>
                        <v-app-bar dense flat>                            
                            <!-- <v-text-field 
                                flat 
                                solo
                                dense
                                readonly
                                single-line
                                hide-details
                                id='copy'
                                :value='shareURL'
                                append-icon="mdi-content-copy"
                                @click:append='copy'>
                            </v-text-field> -->

                            <v-spacer></v-spacer>

                            <l-search @add='addShare'></l-search>

                        </v-app-bar>
                    </template>

                    <!-- <template v-slot:item.builder='{ item }'>
                        <l-share :id='item.id' v-model='items[item.id]' @remove='removeShare'></l-share>
                    </template> -->

                    <template v-slot:item.builder='{ item }'>
                        <l-share :id='item.id' v-model='items[item.id]' @remove='removeShare'></l-share>
                    </template>
                </v-data-table>
            </v-col>
        </v-row>
    </v-container>    
</template>

<script>
import Avatar from '@/shared/Cards/Avatar'
import Search from '@/shared/Dialog/Search'
import Share from './share'

import { access } from '@/config/security'

export default {
    data() {
        return {
            show: false,
            search: "",
            shareURL: "https://layhgobuilder.com/8Vdi0x",
            query: null,
            loading: false,
            options: {},
            items: {},
            headers: [
                 { text: 'builder', value: 'builder' },
            ],
        }
    },
    created() {
        this.initialize();
    },
    props: {
        id: {
            type: String,
            required: true,
            default: function() {
                return this.$route.query.id;
            }
        },
        project: {
            type: Object,
            required: false,
            default: function() {
                return {}
            }
        },
    },
    components: {
        'l-avatar': Avatar,
        'l-share': Share,
        'l-search': Search
    },
    watch: {
        options: {
            handler() {
                this.getBuilders(this.query);
            },
            deep: false
        },
        items(nv, ov) {
            this.project.shared = JSON.parse(JSON.stringify(this.items));
            console.log(this.project.shared);
        }
    },
    computed: {
        builders() {
            let builders = this.$store.getters['auth/builders'];
            return (builders) ? builders : {};
        },
        sharedTotal() {
            return (this.sharedBuilders) ? this.sharedBuilders.length : 0;
        },
        sharedBuilders() {
            let shared = Object.keys(this.items).filter(key => this.items[key] !== "none");
            for (let key in shared) {
                let value = shared[key];
                shared[key] = { id: value }
            }
            return (shared) ? shared : [];
        },
        // shared() {
        //     let shared = this.$store.getters['shared/projects'];
        //     return (shared) ? shared : {};
        // },
    },
    methods: {
        initialize() {
            this.items = Object.assign({}, this.project.shared);
            this.project.shared = JSON.parse(JSON.stringify(this.items));
        },
        reset() {
            this.items = Object.assign({}, this.project.shared);
            this.project.shared = JSON.parse(JSON.stringify(this.items));
            // this.wrapper = Object.assign({}, this.shared[this.id]);
        },
        addShare(item) {
            this.$set(this.items, item.id, access.view.value);
        },
        save() {
            this.project.shared = JSON.parse(JSON.stringify(this.items));
        },
        // async save() {
        //     await this.$store.dispatch('projects/setSharedByProjectId', {
        //         id: this.id,
        //         value: this.project.shared
        //     })
        // },
        removeShare(id) {
            this.$set(this.items, id, "none");
            // this.$delete(this.items, id);
        },
        async getBuilders(query) {
            try {
                this.loading = true;
                let result = await this.$store.dispatch('auth/getBuilders', { query: query, limit: 10 });
                this.loading = false;
                
                if (result.query === undefined) return;
                this.query = result.query;
            } catch(err) {
                console.log(err);
            }
        },
    }
}
</script>