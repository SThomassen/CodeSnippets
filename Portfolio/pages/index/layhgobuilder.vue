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
                                    Camera Movement
                                </v-expansion-panel-header>
                                <v-expansion-panel-content>
                                    <s-cameramovement></s-cameramovement>
                                </v-expansion-panel-content>
                            </v-expansion-panel>
                            <v-expansion-panel>
                                <v-expansion-panel-header>
                                    Command System
                                </v-expansion-panel-header>
                                <v-expansion-panel-content>
                                    <s-commandsystem></s-commandsystem>
                                </v-expansion-panel-content>
                            </v-expansion-panel>
                            <v-expansion-panel>
                                <v-expansion-panel-header>
                                    Fence Connection
                                </v-expansion-panel-header>
                                <v-expansion-panel-content>
                                    <s-fenceconnect></s-fenceconnect>
                                </v-expansion-panel-content>
                            </v-expansion-panel>
                            <v-expansion-panel>
                                <v-expansion-panel-header>
                                    Pool Manager
                                </v-expansion-panel-header>
                                <v-expansion-panel-content>
                                    <s-poolmanager></s-poolmanager>
                                </v-expansion-panel-content>
                            </v-expansion-panel>
                            <v-expansion-panel>
                                <v-expansion-panel-header>
                                    Terrain Controller
                                </v-expansion-panel-header>
                                <v-expansion-panel-content>
                                    <s-terraincontroller></s-terraincontroller>
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
    import { layhgobuilder } from '@/projects/layhgobuilder'
    import Info from '@/components/work/info'

    import CameraMovement from '@/components/snippets/layhgobuilder/CameraMovement'
    import CommandSystem from '@/components/snippets/layhgobuilder/CommandSystem'
    import FenceConnect from '@/components/snippets/layhgobuilder/FenceConnection'
    import PoolManager from '@/components/snippets/layhgobuilder/PoolManager'
    import TerrainController from '@/components/snippets/layhgobuilder/TerrainController'

    export default {
        head() {
            return {
                title: `Layhgobuilder`,
            }
        },
        data() {
            return {
                model: 0,
            }
        },
        components: {
            's-info': Info,
            's-cameramovement': CameraMovement,
            's-commandsystem':  CommandSystem,
            's-fenceconnect': FenceConnect,
            's-poolmanager': PoolManager,
            's-terraincontroller': TerrainController
        },
        computed: {
            project() {
                return layhgobuilder ? layhgobuilder : {};
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