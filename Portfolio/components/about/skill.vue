<template>
    <v-container>
        
        <v-row>
            <v-col cols="12">
                <h3>{{$t("languages")}}</h3>
            </v-col>
            <v-col cols="12" v-for="item in languages" :key="item.name">
                <v-progress-linear height="20" :color="item.color" v-model="item.value">
                    <span class="ml-2 text-caption">{{item.name}}</span>
                    <v-spacer/>
                    <strong class="mr-2 text-caption">{{ Math.ceil(item.value) }}%</strong>
                </v-progress-linear>
            </v-col>
        </v-row>

        <v-row>
            <v-col cols="12">
                <h3>{{$t("programs")}}</h3>
            </v-col>
            <v-col cols="12" v-for="item in programs" :key="item.name">
                <v-progress-linear height="20" :color="item.color" v-model="item.value">
                    <span class="ml-2 text-caption">{{item.name}}</span>
                    <v-spacer/>
                    <strong class="mr-2 text-caption">{{ Math.ceil(item.value) }}%</strong>
                </v-progress-linear>
            </v-col>
        </v-row>

        <v-row>
            <v-col cols="12">
                <v-btn outlined @click="downloadCV">
                    {{$t("download_cv")}}
                </v-btn>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
export default {
    computed: {
        languages() {
            return [
                { name: "C++", value: "80", color: "cyan lighten-2" },
                { name: "C#", value: "90", color: "cyan lighten-2" },
                { name: "DOTS", value: "85", color: "cyan lighten-2" },
                { name: "Vue", value: "80", color: "cyan lighten-2" },
                { name: "Js", value: "90", color: "cyan lighten-2" },
                { name: "HTML/CSS", value: "60", color: "cyan lighten-2" },
            ]
        },
        programs() {
            return [
                { name: "Unity", value: "95", color: "amber darken-1" },
                { name: "Unreal", value: "70", color: "amber darken-1" },
                { name: "FMOD", value: "85", color: "amber darken-1" },
                { name: "Photoshop", value: "80", color: "amber darken-1" },
            ]
        }
    },
    methods: {
        downloadCV() {
            let exportFile = "/downloads/Sjors_Thomassen_CV_2022.pdf";

            let linkElement = document.createElement('a');
            linkElement.setAttribute('href', exportFile);
            linkElement.setAttribute("target", "_blank");
            linkElement.setAttribute("title", "Sjors Thomassen Resume");
            linkElement.click();
        }
    }
}
</script>