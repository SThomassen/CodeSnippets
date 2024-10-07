<template>
    <v-dialog v-model="dialog" max-width="800px">
		<template v-slot:activator="{ on }">
			<v-btn small text color="primary" dark v-on="on">{{ $t('terms_conditions') }}*</v-btn>
		</template>

		
		<v-card>
			<v-card-title>
				<span class="headline">{{$t('terms_conditions') }} Layhgobuilder</span>
			</v-card-title>
			<v-card-text>
				<span v-html='conditions'></span>
			</v-card-text>
			<v-card-actions>
				<v-btn text @click="print">{{ $t('print') }}</v-btn>
				<v-spacer></v-spacer>
                <v-btn color="red darken-1" text @click="decline">{{ $t('disagree') }}</v-btn>
                <v-btn color="green darken-1" text @click="accept">{{ $t('agree') }}</v-btn>
			</v-card-actions>
		</v-card>
    </v-dialog>
</template>

<script>
import terms from '@/config/terms'

export default {
    data () {
      	return {
        	dialog: false,
      	}
    },
    computed: {
		conditions() {
			return terms.conditions;
		}
    },
    methods: {
		print() {
			window.open('assets/Gebruikersvoorwaarden_Layhgo.pdf','_blank'); //,'fullscreen=yes');
		},
		accept() {
			this.$emit('terms',true);
			this.dialog = false;
		},
		decline() {
			this.$emit('terms',false);
			this.dialog = false;
		}
	}
}
</script>