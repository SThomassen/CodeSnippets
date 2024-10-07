<template>
    <v-dialog
        fullscreen
        hide-overlay
        v-model="active"
        transition="dialog-bottom-transition">
        <v-card>
            <v-row no-gutters>
                <v-col lg="8" offset-lg="2" cols="12">

                    <v-row class="mt-4">
                        <v-spacer/>
                        <v-col cols="auto">
                            <v-tooltip bottom>
                                <template v-slot:activator='{ on }'>
                                    <v-btn icon tile v-on="on" @click="toPrevious" :disabled="paginate.previous === null">
                                        <v-icon>mdi-chevron-left</v-icon>
                                    </v-btn>
                                </template>
                                {{paginate.pTag}}
                            </v-tooltip>

                            <v-tooltip bottom>
                                <template v-slot:activator='{ on }'>
                                    <v-btn icon tile v-on="on" @click="toNext" :disabled="paginate.next === null">
                                        <v-icon>mdi-chevron-right</v-icon>
                                    </v-btn>
                                </template>
                                {{paginate.nTag}}
                            </v-tooltip>

                            <v-tooltip bottom>
                                <template v-slot:activator='{ on }'>
                                    <v-btn icon tile v-on="on" @click="close">
                                        <v-icon>mdi-close</v-icon>
                                    </v-btn>
                                </template>
                                {{$t('close')}}
                            </v-tooltip>
                        </v-col>
                    </v-row>

                    <v-row>
                        <v-col cols="12">
                            <nuxt-child />
                        </v-col>
                    </v-row>
                    
                </v-col>
            </v-row>
        </v-card>
    </v-dialog>
</template>

<script>
export default {
    data() {
        return {
            active: false
        }
    },
    computed: {
        path() {
            let path = this.$route.name.substring(this.$route.name.indexOf("-") + 1);
			path = path.slice(0, path.indexOf("__"));

            return path;
        },
        paginate() {
            switch(this.path) {
                case "histera": return { previous: null, next: "/layhgobuilder", pTag: "", nTag: "Layhgobuilder" };
                case "layhgobuilder": return { previous: "/histera", next: "/recovry", pTag: "Histera", nTag: "RecoVRy" };
                case "recovry": return { previous: "/layhgobuilder", next: "/cross_the_river", pTag: "Layhgobuilder", nTag: "Cross the River" };
                case "cross_the_river": return { previous: "/recovry", next: "/one_eyed_pirate", pTag: "RecoVRy", nTag: "One Eyed Pirate" };
                case "one_eyed_pirate": return { previous: "/cross_the_river", next: "/free_fowling", pTag: "Cross the River", nTag: "Free Fowling" };
                case "free_fowling": return { previous: "/one_eyed_pirate", next: "/runner", pTag: "One Eyed Pirate", nTag: "Runner" };
                case "runner": return { previous: "/free_fowling", next: "/samsung", pTag: "Free Fowling", nTag: "Samsung VR Jam" };
                case "samsung": return { previous: "/runner", next: "/automatician", pTag: "Runner", nTag: "Automatician" };
                case "automatician": return { previous: "/samsung", next: "/bolt_storm", pTag: "Samsung VR Jam", nTag: "Bolt Storm" };
                case "bolt_storm": return { previous: "/automatician", next: "/beru", pTag: "Automatician", nTag: "Beru" };
                case "beru": return { previous: "/bolt_storm", next: "/elements", pTag: "Bolt Storm", nTag: "Elements" };
                case "elements": return { previous: "/beru", next: "/dargon_lair", pTag: "Beru", nTag: "Dargon Lair" };
                case "dargon_lair": return { previous: "/elements", next: null, pTag: "Elements", nTag: "" };
                default: return { previous: undefined, next: undefined };
            }
        },
    },
    methods: {
        open(route) {
            let path = this.localePath(route)
            this.$router.push(path);
            this.active = true
        },
        close() {
            this.active = false;
            let path = this.localePath("/")
            this.$router.push(path);
        },
        
        toPrevious() {
            let path = this.localePath(this.paginate.previous);
            this.$router.push(path);
        },
        toNext() {
            let path = this.localePath(this.paginate.next);
            this.$router.push(path);
        }
    }
}
</script>

<style>
.dialog-bottom-transition-enter-active {
  transition: all .5s ease-out;
}

.dialog-bottom-transition-leave-active {
  transition: all .5s cubic-bezier(1, 0.5, 0.8, 1);
}
</style>