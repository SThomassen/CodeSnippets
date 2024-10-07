<template>
    <v-container>
        <v-row>
            <v-col cols="12">
                <v-list>
                    <template v-for="item in info">
                        <v-list-item :key="item[0]" two-line>
                            <v-list-item-content>
                                <v-list-item-title>
                                    {{$t(item[0])}}
                                </v-list-item-title>
                                <v-list-item-subtitle v-html="item[1]">
                                </v-list-item-subtitle>
                            </v-list-item-content>
                        </v-list-item>
                    </template>

                    <v-list-item>
                        <v-list-item-content>
                            <v-list-item-title>
                                {{$t("about")}}
                            </v-list-item-title>
                            <v-list-item-subtitle class="text-wrap">
                                {{$t("about_me")}}
                            </v-list-item-subtitle>
                        </v-list-item-content>
                    </v-list-item>
                </v-list>
            </v-col>
        </v-row>
    </v-container>
</template>

<script>
export default {
    computed: {
        info() {
            return Object.entries( {
                name: "Sjors Thomassen",
                email: "sjors2505@gmail.com",
                age: this.age,
                residence: this.residence,
                education: "Creative Media & Game Technologies",
                profession: this.$t("game_programmer"),
                hobby: "Vinyl DJ",
                games: "Sea of Thieves <br/>Assassins Creed - Black Flag<br/>LEGO Games"
            })
        },
        age() {
            var today = new Date();
            var birthDate = new Date("1991-5-25");
            var age = today.getFullYear() - birthDate.getFullYear();
            var m = today.getMonth() - birthDate.getMonth();
            if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
                age--;
            }
            return age;
        },
        residence() {
            return `Breda, ${this.$t('netherlands')}`;
        }
    }
}
</script>