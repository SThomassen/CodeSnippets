<template>
    <v-list-item two-line>    
        <v-list-item-content>
            <v-list-item-title>
                {{report.title}}
                <v-chip dark class='ml-2' x-small :color='typeColor'>{{ $t(`reports.types.content.${report.type}.label`) }}</v-chip>
            </v-list-item-title>
            <v-list-item-subtitle>
                <l-timestamp :date='report.created' format='short'></l-timestamp> by {{report.authorName}}<!--D MMMM YYYY-->
            </v-list-item-subtitle>
        </v-list-item-content>

        <v-list-item-action>
            <v-chip dark :color='stateColor'>{{ $t(`reports.states.content.${report.state}`) }}</v-chip>
        </v-list-item-action>                                    
    </v-list-item>
</template>

<script>
import Timestamp from '@/shared/Input/Timestamp'
import info from '@/config/report'

export default {
    components: {
        'l-timestamp': Timestamp
    },
    props: {
        report: {
            type: Object,
            required: true,
            default: function() {
                return {}
            }
        }
    },
    computed: {
        stateColor() {
            let state = info.states.find(item => item.id === this.report.state);
            return state?.color;
        },
        typeColor() {
            let type = info.types.find(item => item.id === this.report.type);
            return type?.color;
        },
    }
}
</script>