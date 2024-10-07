<template>
    <v-dialog 
        v-model='dialog'
        max-width='800px'
        transition="dialog-bottom-transition"
        @keydown.esc="cancel"
        @click:outside='cancel' >
        <v-card>
            <v-app-bar flat>
                <v-btn icon @click='cancel'>
                    <v-icon>mdi-close</v-icon>
                </v-btn>

                <v-toolbar-title>{{$t('reports.states.content.duplicate')}}</v-toolbar-title>
                <v-spacer></v-spacer>

            <v-btn outlined color='info'>{{$t('save')}}</v-btn>
            </v-app-bar>

            <v-data-table :headers='headers' :items='reports'>
                <template v-slot:item.actions='{item}'>
                    <v-tooltip right>
                        <template v-slot:activator='{ on }'>
                            <v-btn icon v-on='on'>
                                <v-icon>mdi-folder-open</v-icon>
                            </v-btn>
                        </template>
                        {{$t('open')}}
                    </v-tooltip>
                    <v-tooltip right>
                        <template v-slot:activator='{ on }'>
                            <v-btn icon v-on='on' @click.prevent='select(item)'>
                                <v-icon>mdi-arrow-right</v-icon>
                            </v-btn>
                        </template>
                        {{$t('select')}}
                    </v-tooltip>
                </template>
            </v-data-table>
        </v-card>
    </v-dialog>
</template>

<script>
export default {
    data() {
        return {
            dialog: true,
            headers: [
                { value: "number", text: "Nr.", width:"10%" },
                { value: "title", text: "Title", width: "70%" },
                { value: "actions", text: "", width: "20%"}
            ]
        }
    },
    props: {
        report: {
            type: Object,
            required: true,
            default: function() {
                return {};
            }
        }
    },
    computed: {
        reports() {
            return Object.values(this.$store.getters['reports/reports']).filter(report => report.id !== this.report.id);
        },
    },
    methods: {
        open() {
            this.dialog = true;
        },  
        cancel() {
            this.dialog = false;
        },
        select(item) {
            this.$emit("onSelect", { id: item.id, title: item.title, number: item.number});
            this.dialog = false;
        },
        async getReports() {

        }
    }
}
</script>