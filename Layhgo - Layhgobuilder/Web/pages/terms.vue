<template>
    <v-container>
        <l-scrolltotop :y='offsetTop'></l-scrolltotop>

        <v-row justify="end" class='mt-2'>
            <v-btn outlined class='mb-3' @click='print'>
                <v-icon left>mdi-printer</v-icon>
                {{$t('print')}}
            </v-btn>
        </v-row>
        <span v-html='conditions' v-scroll='onScroll'></span>
        
    </v-container>
</template>

<script>
import terms from '@/config/terms'
import ScrollToTop from '@/shared/Observers/ScrollToTop'

export default {
    head() {
        return {
            title: `Layhgobuilder | Terms & Conditions`,
        }
    },
    data() {
        return {
            offsetTop:0,
        }
    },
    components: {
        'l-scrolltotop': ScrollToTop
    },
    computed: {
		conditions() {
			return terms.conditions;
        },
    },
    methods: {
        print() {
            window.open('Gebruikersvoorwaarden_Layhgobuilder.pdf','_blank');
        },
        onScroll() {
            this.offsetTop = window.scrollY;
        }
    }
}
</script>