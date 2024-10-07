<template>
    <v-container>
        <v-row>
            <v-col lg="8" cols="12">
                <v-carousel v-model="model" :height="imageHeight">
                    <v-carousel-item
                        v-for="(item) in project.images"
                        :key="item">
                        <v-img
                            contain
                            :src="item"
                            height="100%"
                            tile>
                        </v-img>
                    </v-carousel-item>
                </v-carousel>

                <v-row class="mt-12">
                    <v-col class="box line-numbers">
                        <v-expansion-panels flat>
                            <v-expansion-panel>
                                <v-expansion-panel-header>
                                    Pathfinding
                                </v-expansion-panel-header>
                                <v-expansion-panel-content>
                                    <s-snippet></s-snippet>
                                </v-expansion-panel-content>
                            </v-expansion-panel>
                        </v-expansion-panels>
                    </v-col>
                </v-row>
            </v-col>

            <v-col lg="4" cols="12">
                <s-info :project="project"></s-info>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
    import { cross_river } from '@/projects/cross_the_river';
    import Info from '@/components/work/info'
    import snippets from '@/components/snippets/CrossTheRiver/Pathfinder'
    
    export default {
        head() {
            return {
                title: `Cross the River`,
            }
        },
        data() {
            return {
                model: 0,
            }
        },
        components: {
            's-info': Info,
            's-snippet': snippets
        },
        computed: {
            project() {
                return cross_river ? cross_river : {};
            },
            imageHeight () {
                switch (this.$vuetify.breakpoint.name) {
                    case 'xs': return '300px'
                    case 'sm': return '400px'
                    case 'md': return '500px'
                    case 'lg': return '400px'
                    case 'xl': return '540px'
                }
            },
        },
    }
</script>