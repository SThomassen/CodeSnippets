<template>
    <dashboard :id="'dashExample'">
        <dash-layout v-for="layout in work" v-bind="layout" :debug="false" :key="layout.breakpoint">
            <dash-item v-for="(item,n) in layout.items" v-bind.sync="item" :key="item.id" class="pa-2">
                <v-hover v-slot="{ hover }">
                    <v-card
                        @click="$bus.$emit('openProject', item.route)"
                        :elevation="hover ? '8' : '0'"
                        style="transition: transform 300ms"
                        class="content item-move">
                        <v-img
                            dark
                            width="100%"
                            height="100%"
                            :src="cover(item)"
                            :lazy-src="`https://picsum.photos/10/6?image=${n * 5 + 10}`"
                            :class="`grey lighten-2 ${hover ? 'image-scale' : ''}`">
                            <template v-slot:placeholder>
                                <v-row
                                    class="fill-height ma-0"
                                    align="center"
                                    justify="center">
                                    <v-progress-circular
                                        indeterminate
                                        color="grey lighten-5">
                                    </v-progress-circular>
                                </v-row>
                            </template>
                            <v-expand-transition>
                                <div
                                    v-if="hover"
                                    class="d-flex transition-fast-in-fast-out v-card--reveal text-h2 white--text"
                                    style="height: 72px;">
                                    <v-card width="100%" height="100%">
                                        <v-list-item two-line>
                                            <v-list-item-content>
                                                <v-list-item-title class="title">{{title(item)}}</v-list-item-title>
                                                <v-list-item-subtitle>{{date(item)}}</v-list-item-subtitle>
                                            </v-list-item-content>
                                        </v-list-item>
                                    </v-card>
                                </div>
                            </v-expand-transition>
                        </v-img>
                    </v-card>
                </v-hover>            
            </dash-item>
        </dash-layout>
    </dashboard>
</template>

<script>
    import { Dashboard, DashLayout, DashItem } from "vue-responsive-dash";
    import config from '@/config/projects'
    import Dialog from './dialog.vue'
    
    export default {
        data() {
            return {
    
            }
        },
        components: {
            Dashboard,
            DashLayout,
            DashItem,
            "s-dialog": Dialog
        },
        computed: {
            work() {
                return config.layouts;
            },
            lang() {
                return this.$i18n.locale;
            }
        },
        methods: {
            title(item) {
                return item.project?.title;
            },
            cover(item) {
                return item.project?.cover;
            },
            date(item) {
                return item.project?.date;
            }
        }
    }
</script>
    
<style>
    .v-card--reveal {
        align-items: center;
        bottom: 0;
        justify-content: center;
        opacity: .5;
        position: absolute;
        width: 100%;
    }

    .item-move {
        transition: all .5s cubic-bezier(.55,0,.1,1);
        -webkit-transition: all .5s cubic-bezier(.55,0,.1,1);
    }

    .content {
        height: 100%;
        width: 100%;
        border: 2px solid #42b983;
        border-radius: 50px;
    }

    .image-scale {
        transition: all 0.2s;
        transform: scale(1.01);
    }
</style>