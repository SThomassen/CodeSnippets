<template>
    <v-dialog
        v-model="dialog"
        fullscreen
        hide-overlay
        transition="dialog-bottom-transition">
        <template v-slot:activator="{ on, attrs }">
            <v-btn
            color="primary"
            dark
            v-bind="attrs"
            v-on="on">
            Open Dialog
            </v-btn>
        </template>
        <v-card>
            <v-toolbar>
                <v-btn
                    icon
                    dark
                    @click="dialog = false">
                    <v-icon>mdi-close</v-icon>
                </v-btn>
                <v-toolbar-title>{{$t('purchase')}}</v-toolbar-title>
            </v-toolbar>

            <v-container fluid>
                <v-row>
                    <v-col cols="8" offset="2">
                        <v-stepper v-model="stepper">
                            <v-stepper-header>
                                <v-stepper-step v-for="(item,i) in headers" :key="i" :complete="stepper > i" :step="i">
                                    {{$t(item.title)}}
                                </v-stepper-step>
                            </v-stepper-header>

                            <v-stepper-items>
                                <v-stepper-content step="1">
                                    <v-card>
                                        <v-list>
                                            <v-list-item>
                                                <v-select dense :items="['Basic','Pro']"></v-select>
                                            </v-list-item>
                                        </v-list>
                                    </v-card>

                                    <v-btn color="primary" @click="stepper = 2">
                                        Continue
                                    </v-btn>

                                    <v-btn text>
                                        Cancel
                                    </v-btn>
                                </v-stepper-content>

                                <v-stepper-content step="2">
                                    <v-card
                                        class="mb-12"
                                        color="grey lighten-1"
                                        height="200px">
                                    </v-card>

                                    <v-btn color="primary" @click="stepper = 3">
                                        Continue
                                    </v-btn>

                                    <v-btn text>
                                        Cancel
                                    </v-btn>
                                </v-stepper-content>

                                <v-stepper-content step="3">
                                    <v-card
                                        class="mb-12"
                                        color="grey lighten-1"
                                        height="200px">
                                    </v-card>

                                    <v-btn color="primary" @click="stepper = 1">
                                        Continue
                                    </v-btn>

                                    <v-btn text>
                                        Cancel
                                    </v-btn>
                                </v-stepper-content>
                            </v-stepper-items>
                        </v-stepper>
                    </v-col>
                </v-row>
            </v-container>
        </v-card>
    </v-dialog>
</template>
    
<script>
export default {
    data () {
        return {
            dialog: true,
            stepper: "1",
            headers: [
                {
                    title: 'configure'
                },
                {
                    title: 'signin'
                },
                {
                    title: 'payment'
                }
            ]
        }
    },
}
</script>