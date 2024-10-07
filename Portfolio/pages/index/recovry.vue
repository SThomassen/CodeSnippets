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
            </v-col>

            <v-col lg="4" cols="12">
                <s-info :project="project"/>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
    import { recovry } from '@/projects/recovry';
    import Info from '@/components/work/info'
    
    export default {
        head() {
            return {
                title: `RecoVRy`,
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
                return recovry ? recovry : {};
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
        }
    }
</script>