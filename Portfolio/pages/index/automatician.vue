<template>
    <v-container>
        <v-row>
            <v-col lg="8" cols="12">

                <v-row>
                    <v-col cols="12">
                        <v-carousel v-model="model" :height="imageHeight">
                            <v-carousel-item
                                v-for="(item) in project.images"
                                :key="item">
                                <v-img
                                    tile
                                    contain
                                    :src="item"
                                    height="100%">
                                </v-img>
                            </v-carousel-item>
                        </v-carousel>
                    </v-col>
                    <v-col cols="12">
                        <iframe width="100%" height="600" src="https://www.youtube.com/embed/MtJW4DkKlJg?controls=0" title="Automatician" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
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
    import { automatician } from '@/projects/automatician';
    import Info from '@/components/work/info'
    
    export default {
        head() {
            return {
                title: `The Automatician`,
            }
        },
        data() {
            return {
                model: 0,
            }
        },
        components: {
            's-info': Info
        },
        computed: {
            project() {
                return automatician ? automatician : {};
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