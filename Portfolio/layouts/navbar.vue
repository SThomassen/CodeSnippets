<template>
    <nav>
        <v-app-bar
            app
            elevation="0" 
            color="transparent">

            <v-container>
                <v-row>
                    <v-col cols="12" sm="10" offset-sm="1">
                        <v-row justify="center" align="center">
                            
                            <v-col cols="auto">
                                <v-toolbar-title headline text-uppercase>
                                    <router-link :to="localePath('/')" tag='span' style='cursor: pointer'>
                                        <v-tooltip right>
                                            <template v-slot:activator='{ on }'>
                                                <div v-on='on'>
                                                    <span :class='`${$vuetify.breakpoint.xs ? "caption" : "display-1"} font-weight-light`'>Sjors</span>
                                                    <span class='caption font-weight-light'>Thomassen</span>
                                                </div>
                                            </template>
                                            <span>{{ $t("home") }}</span>
                                        </v-tooltip>
                                    </router-link>
                                    
                                </v-toolbar-title>
                            </v-col>

                            <v-spacer/>

                            <v-col cols="auto">
                                <s-navigation></s-navigation>
                            </v-col>
                        </v-row>
                    </v-col>
                </v-row>
            </v-container>
        </v-app-bar>
    </nav>
</template>

<script>
import Navigation from "@/components/navbar/navigation"

export default {
    data() {
        return {
            transparent: true
        }
    },
    components: {
        "s-navigation": Navigation
    },
    computed: {
        onScroll(e) {
            this.transparent = !(e.target.scrollTop > 0);
        }
    }
}
</script>