<template>
    <v-container>
        <v-row class='mt-2'>
            <v-col>
                <span class='text-h2'>FAQ</span>
                <v-divider></v-divider>

                <v-row>
                    <v-col cols='12' class='pa-2 mt-4'>
                        <v-expansion-panels flat>
                            <v-expansion-panel v-for='(q,i) in questions' :key='i'>
                                <v-expansion-panel-header>
                                    <span class='subtitle-2'>{{q.question}}</span>
                                </v-expansion-panel-header>
                                <v-expansion-panel-content>
                                    <p>{{q.answer}}</p>
                                </v-expansion-panel-content>
                            </v-expansion-panel>
                        </v-expansion-panels>
                    </v-col>
                </v-row>

            </v-col>
        </v-row>
    </v-container>
</template>

<script>
export default {
    head() {
        return {
            title: `Layhgobuilder | FAQ`,
        }
    },
    data() {
        return {
            questions: [
                { 
                    question: "I had a pop up saying that I ran out of memory.",
                    answer: "Unfortunately the application can break occasionally due to memory limitations. We are still in development, this is a beta version of Layhgobuilder. We recommend you to save your project regularly."
                },
                {
                    question: "How can I add my products to Layhgobuilder?",
                    answer: "We're always looking for new suppliers to add their objects to our application. For more information, message us at universal@layhgobuilder.com."
                }
            ]
        }
    }
}
</script>