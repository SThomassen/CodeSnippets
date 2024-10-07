<template>
    <v-container>
        <v-row>
            <v-col lg="8" cols="12">
                <v-carousel v-model="model" height="600px">
                    <v-carousel-item
                        v-for="(item, i) in project.images"
                        :key="item">
                        <v-img
                            contain
                            :src="item"
                            height="100%"
                            tile>
                        </v-img>
                    </v-carousel-item>
                </v-carousel>
            </v-col>

            <v-col lg="4" cols="12">
                <s-info :project="project"/>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
    import { dargon_lair } from '@/projects/dargon_lair';
    import Info from '@/components/work/info'
    
    export default {
        head() {
            return {
                title: `Dargon Lair`,
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
                return dargon_lair ? dargon_lair : {};
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