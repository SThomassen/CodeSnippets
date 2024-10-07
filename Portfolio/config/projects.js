import { histera } from '@/projects/histera'
import { layhgobuilder } from '@/projects/layhgobuilder'
import { recovry } from '@/projects/recovry'
import { free_fowling } from '@/projects/free_fowling'
import { cross_river } from '@/projects/cross_the_river'
import { one_eyed_pirate } from '@/projects/one_eyed_pirate'
import { runner } from '@/projects/runner'
import { samsung } from '@/projects/samsung'
import { automatician } from '@/projects/automatician'
import { boltstorm } from '@/projects/boltstorm'
import { beru } from '@/projects/beru'
import { elements } from '@/projects/elements'
import { dargon_lair } from '@/projects/dargon_lair'

export default {
    layouts: [
        {
            breakpoint: "xl",
            numberOfCols: 12,
            items: [
                { 
                    id: "12", 
                    x:8, y:10, 
                    width: 2, height: 2, 
                    resizable: false, draggable: false, 
                    title: "Dargon Lair",
                    coverPath: "images/dargon_lair/dargon_01.jpg",
                    route: "/dargon_lair"
                },
                { 
                    id: "11", 
                    x:6, y:10, 
                    width: 2, height: 2, 
                    resizable: false, draggable: false, 
                    title: "Elements",
                    coverPath: "images/elements/elements_01.jpg",
                    route: "/elements"
                },
                { 
                    id: "10", 
                    x:3, y:10, 
                    width: 3, height: 2, 
                    resizable: false, draggable: false, 
                    title: "Beru",
                    coverPath: "images/beru/beru_01.jpg",
                    route: "/beru"
                },
                { 
                    id: "9", 
                    x: 0, y: 10, 
                    width: 3, height: 2, 
                    resizable: false, draggable: false, 
                    title: "Bolt Storm",
                    coverPath: "images/bolt_storm/boltstorm_01.jpg",
                    route: "/bolt_storm"
                },
                { 
                    id: "8", 
                    x: 5, y: 7, 
                    width: 5, height: 3, 
                    resizable: false, draggable: false, 
                    title: "The Automatician",
                    coverPath: "images/automatician/automatician_01.png",
                    route: "/automatician"
                },
                { 
                    id: "7", 
                    x: 0, y: 7, 
                    width: 5, height: 3, 
                    resizable: false, draggable: false, 
                    title: "Samsung VR Jam",
                    coverPath: "images/samsung/VRJam.png",
                    route: "/samsung"
                },
                { 
                    id: "6", 
                    x: 6, y: 3,
                    width: 4, height: 4, 
                    resizable: false, draggable: false, 
                    title: "Runner",
                    coverPath: "images/runner/runner_01.png",
                    route: "/runner"
                },
                { 
                    id: "5", 
                    x: 2, y: 5, 
                    width: 4, height: 2, 
                    resizable: false, draggable: false, 
                    title: "Free Fowling",
                    coverPath: "images/free_fowling/alakajam_1.png",
                    route: "/free_fowling"
                },
                {
                    id: "4", 
                    x: 2, y: 3, 
                    width: 4, height: 2, 
                    resizable: false, draggable: false, 
                    title: "One Eyed Pirate",
                    coverPath: "images/one_eyed_pirate/ship_01.jpg",
                    route: "/one_eyed_pirate"
                },
                { 
                    id: "3", 
                    x: 0, y: 3, 
                    width: 2, height: 4, 
                    resizable: false, draggable: false, 
                    title: "Cross the River",
                    coverPath: "images/cross_the_river/river_01.jpg",
                    route: "/cross_the_river"
                },
                { 
                    id: "2", 
                    x:7, y:0, 
                    width: 3, height: 3, 
                    resizable: false, draggable: false, 
                    title: "RecoVRy",
                    coverPath: "images/recovry/recovry_1.jpg",
                    route: "/recovry"
                },
                { 
                    id: "1", 
                    x:0, y:0, 
                    width: 7, height: 3, 
                    resizable: false, draggable: false, 
                    title: "Layhgobuilder",
                    coverPath: "images/layhgobuilder/layhgobuilder_1.png",
                    route: "/layhgobuilder",
                    description: "Layhgobuilder is a program to design your own festivals. I have been developing the application for 4 years total. For 2 years the focus was on the website and a web application. After the 2 years I continued the development with a standalone version with more features and content. You can try the standalone version on request."
                },
                {
                    id: "0",
                    x: 0, y: 0,
                    width: 7, height: 3,
                    resizable: false, draggable: false,
                    title: "Histera",
                    coverPath: "images/histera/histera_cover.png",
                    route: "/histera",
                    description: "Histera is a fast pace first person arena shooter, where sectors of the arena glitches to different eras while you are battling with your team against other players."
                }
            ]
        },
        {
            breakpoint: "lg",
            breakpointWidth: 1200,
            numberOfCols: 10,
            items: [
                { 
                    id: "12", 
                    x:8, y:10, 
                    width: 2, height: 2, 
                    resizable: false, draggable: false, 
                    project: dargon_lair,
                    route: "/dargon_lair"
                },
                { 
                    id: "11", 
                    x:6, y:10, 
                    width: 2, height: 2, 
                    resizable: false, draggable: false, 
                    project: elements,
                    route: "/elements"
                },
                { 
                    id: "10", 
                    x:3, y:10, 
                    width: 3, height: 2, 
                    resizable: false, draggable: false, 
                    project: beru,
                    route: "/beru"
                },
                { 
                    id: "9", 
                    x: 0, y: 10, 
                    width: 3, height: 2, 
                    resizable: false, draggable: false, 
                    project: boltstorm,
                    route: "/bolt_storm"
                },
                { 
                    id: "8", 
                    x: 5, y: 7, 
                    width: 5, height: 3, 
                    resizable: false, draggable: false, 
                    project: automatician,
                    route: "/automatician"
                },
                { 
                    id: "7", 
                    x: 0, y: 7, 
                    width: 5, height: 3, 
                    resizable: false, draggable: false, 
                    project: samsung,
                    route: "/samsung"
                },
                { 
                    id: "6", 
                    x: 6, y: 3,
                    width: 4, height: 4, 
                    resizable: false, draggable: false, 
                    project: runner,
                    route: "/runner"
                },
                { 
                    id: "5", 
                    x: 2, y: 5, 
                    width: 4, height: 2, 
                    resizable: false, draggable: false, 
                    project: free_fowling,
                    route: "/free_fowling"
                },
                {
                    id: "4", 
                    x: 2, y: 3, 
                    width: 4, height: 2, 
                    resizable: false, draggable: false, 
                    project: one_eyed_pirate,
                    route: "/one_eyed_pirate"
                },
                { 
                    id: "3", 
                    x: 0, y: 3, 
                    width: 2, height: 4, 
                    resizable: false, draggable: false, 
                    project: cross_river,
                    route: "/cross_the_river"
                },
                { 
                    id: "2", 
                    x:7, y:0, 
                    width: 3, height: 3, 
                    resizable: false, draggable: false, 
                    project: recovry,
                    route: "/recovry"
                },
                { 
                    id: "1", 
                    x:0, y:0, 
                    width: 7, height: 3, 
                    resizable: false, draggable: false, 
                    project: layhgobuilder,
                    route: "/layhgobuilder",
                },
                {
                    id: "0",
                    x: 0, y: 0,
                    width: 10, height: 4,
                    resizable: false, draggable: false,
                    project: histera,
                    route: "/histera",
                }
            ]
        },
        {
            breakpoint: "md",
            breakpointWidth: 996,
            numberOfCols: 12,
            items: [
                { 
                    id: "12", 
                    x:8, y:10, 
                    width: 2, height: 2, 
                    resizable: false, draggable: false, 
                    project: dargon_lair,
                    route: "/dargon_lair"
                },
                { 
                    id: "11", 
                    x:6, y:10, 
                    width: 2, height: 2, 
                    resizable: false, draggable: false, 
                    project: elements,
                    route: "/elements"
                },
                { 
                    id: "10", 
                    x:3, y:10, 
                    width: 3, height: 2, 
                    resizable: false, draggable: false, 
                    project: beru,
                    route: "/beru"
                },
                { 
                    id: "9", 
                    x: 0, y: 10, 
                    width: 3, height: 2, 
                    resizable: false, draggable: false, 
                    project: boltstorm,
                    route: "/bolt_storm"
                },
                { 
                    id: "8", 
                    x: 5, y: 7, 
                    width: 5, height: 3, 
                    resizable: false, draggable: false, 
                    project: automatician,
                    route: "/automatician"
                },
                { 
                    id: "7", 
                    x: 0, y: 7, 
                    width: 5, height: 3, 
                    resizable: false, draggable: false, 
                    project: samsung,
                    route: "/samsung"
                },
                { 
                    id: "6", 
                    x: 6, y: 3,
                    width: 4, height: 4, 
                    resizable: false, draggable: false, 
                    project: runner,
                    route: "/runner"
                },
                { 
                    id: "5", 
                    x: 2, y: 5, 
                    width: 4, height: 2, 
                    resizable: false, draggable: false, 
                    project: free_fowling,
                    route: "/free_fowling"
                },
                {
                    id: "4", 
                    x: 2, y: 3, 
                    width: 4, height: 2, 
                    resizable: false, draggable: false, 
                    project: one_eyed_pirate,
                    route: "/one_eyed_pirate"
                },
                { 
                    id: "3", 
                    x: 0, y: 3, 
                    width: 2, height: 4, 
                    resizable: false, draggable: false, 
                    project: cross_river,
                    route: "/cross_the_river"
                },
                { 
                    id: "2", 
                    x:7, y:0, 
                    width: 3, height: 3, 
                    resizable: false, draggable: false, 
                    project: recovry,
                    route: "/recovry"
                },
                { 
                    id: "1", 
                    x:0, y:0, 
                    width: 2, height: 2, 
                    resizable: false, draggable: false, 
                    project: layhgobuilder,
                    route: "/layhgobuilder",
                },
                {
                    id: "0",
                    x: 0, y: 0,
                    width: 7, height: 3,
                    resizable: false, draggable: false,
                    project: histera,
                    route: "/histera",
                }
            ]
        },
        {
            breakpoint: "sm",
            breakpointWidth: 768,
            numberOfCols: 12,
            items: [
                { 
                    id: "12", 
                    x:0, y:13, 
                    width: 7, height: 3, 
                    resizable: false, draggable: false, 
                    project: dargon_lair,
                    title: "Dargon Lair",
                    coverPath: "images/dargon_lair/dargon_01.jpg",
                    route: "/dargon_lair"
                },
                { 
                    id: "11", 
                    x:0, y:13, 
                    width: 7, height: 3, 
                    resizable: false, draggable: false, 
                    project: elements,
                    title: "Elements",
                    coverPath: "images/elements/elements_01.jpg",
                    route: "/elements"
                },
                { 
                    id: "10", 
                    x:7, y:10, 
                    width: 5, height: 6, 
                    resizable: false, draggable: false, 
                    project: beru,
                    title: "Beru",
                    coverPath: "images/beru/beru_01.jpg",
                    route: "/beru"
                },
                { 
                    id: "9", 
                    x: 0, y: 7, 
                    width: 12, height: 3, 
                    resizable: false, draggable: false, 
                    project: boltstorm,
                    title: "Bolt Storm",
                    coverPath: "images/bolt_storm/boltstorm_01.jpg",
                    route: "/bolt_storm"
                },
                { 
                    id: "8", 
                    x: 4, y: 7, 
                    width: 8, height: 4, 
                    resizable: false, draggable: false, 
                    project: automatician,
                    title: "The Automatician",
                    coverPath: "images/automatician/automatician_01.png",
                    route: "/automatician"
                },
                { 
                    id: "7", 
                    x: 0, y: 3, 
                    width: 4, height: 4, 
                    resizable: false, draggable: false, 
                    project: samsung,
                    title: "Samsung VR Jam",
                    coverPath: "images/samsung/VRJam.png",
                    route: "/samsung"
                },
                { 
                    id: "6", 
                    x: 9, y: 3,
                    width: 3, height: 4, 
                    resizable: false, draggable: false, 
                    project: runner,
                    title: "Runner",
                    coverPath: "images/runner/runner_01.png",
                    route: "/runner"
                },
                { 
                    id: "5", 
                    x: 5, y: 3, 
                    width: 4, height: 4, 
                    resizable: false, draggable: false, 
                    project: free_fowling,
                    title: "Free Fowling",
                    coverPath: "images/free_fowling/alakajam_1.png",
                    route: "/free_fowling"
                },
                {
                    id: "4", 
                    x: 5, y: 3, 
                    width: 7, height: 3, 
                    resizable: false, draggable: false, 
                    project: one_eyed_pirate,
                    title: "One Eyed Pirate",
                    coverPath: "images/one_eyed_pirate/ship_01.jpg",
                    route: "/one_eyed_pirate"
                },
                { 
                    id: "3", 
                    x: 0, y: 1, 
                    width: 5, height: 7, 
                    resizable: false, draggable: false, 
                    project: cross_river,
                    title: "Cross the River",
                    coverPath: "images/cross_the_river/river_01.jpg",
                    route: "/cross_the_river"
                },
                { 
                    id: "2", 
                    x:8, y:0, 
                    width: 4, height: 5, 
                    resizable: false, draggable: false, 
                    project: recovry,
                    route: "/recovry"
                },
                { 
                    id: "1", 
                    x:0, y:0, 
                    width: 8, height: 5, 
                    resizable: false, draggable: false, 
                    project: layhgobuilder,
                    route: "/layhgobuilder",
                },
                {
                    id: "0",
                    x: 0, y: 0,
                    width: 12, height: 5,
                    resizable: false, draggable: false,
                    project: histera,
                    route: "/histera",
                }
            ]
        },
        {
            breakpoint: "xs",
            breakpointWidth: 480,
            numberOfCols: 4,
            items: [
                { 
                    id: "12", 
                    x:2, y:0, 
                    width: 2, height: 2, 
                    resizable: false, draggable: false, 
                    project:dargon_lair,
                    route: "/dargon_lair"
                },
                { 
                    id: "11", 
                    x:0, y:0, 
                    width: 2, height: 2, 
                    resizable: false, draggable: false, 
                    project: elements,
                    route: "/elements"
                },
                { 
                    id: "10", 
                    x:0, y:0, 
                    width: 4, height: 2, 
                    resizable: false, draggable: false, 
                    project: beru,
                    route: "/beru"
                },
                { 
                    id: "9", 
                    x: 0, y: 0, 
                    width: 4, height: 2, 
                    resizable: false, draggable: false, 
                    project: boltstorm,
                    route: "/bolt_storm"
                },
                { 
                    id: "8", 
                    x: 0, y: 0, 
                    width: 4, height: 2, 
                    resizable: false, draggable: false, 
                    project: automatician,
                    route: "/automatician"
                },
                { 
                    id: "7", 
                    x: 2, y: 0, 
                    width: 2, height: 2, 
                    resizable: false, draggable: false, 
                    project: samsung,
                    route: "/samsung"
                },
                { 
                    id: "6", 
                    x: 0, y: 0,
                    width: 2, height: 2, 
                    resizable: false, draggable: false, 
                    project: runner,
                    route: "/runner"
                },
                { 
                    id: "5", 
                    x: 2, y: 0, 
                    width: 2, height: 2, 
                    resizable: false, draggable: false, 
                    project: free_fowling,
                    route: "/free_fowling"
                },
                {
                    id: "4", 
                    x: 2, y: 0, 
                    width: 2, height: 2, 
                    resizable: false, draggable: false, 
                    project: one_eyed_pirate,
                    route: "/one_eyed_pirate"
                },
                { 
                    id: "3", 
                    x: 0, y: 0, 
                    width: 2, height: 4, 
                    resizable: false, draggable: false, 
                    project: cross_river,
                    route: "/cross_the_river"
                },
                { 
                    id: "2", 
                    x:0, y:0, 
                    width: 4, height: 2, 
                    resizable: false, draggable: false, 
                    project: recovry,
                    route: "/recovry"
                },
                { 
                    id: "1", 
                    x:0, y:0, 
                    width: 4, height: 3, 
                    resizable: false, draggable: false, 
                    project: layhgobuilder,
                    route: "/layhgobuilder",
                },
                {
                    id: "0",
                    x: 0, y: 0,
                    width: 4, height: 2,
                    resizable: false, draggable: false,
                    project: histera,
                    route: "/histera",
                }
            ]
        },
        {
            breakpoint: "xxs",
            breakpointWidth: 0,
            numberOfCols: 1,
            items: [
                { 
                    id: "12", 
                    x: 0, y: 0, 
                    width: 1, height: 1, 
                    resizable: false, draggable: false, 
                    project: dargon_lair,
                    route: "/dargon_lair"
                },
                { 
                    id: "11", 
                    x: 0, y: 0, 
                    width: 1, height: 1, 
                    resizable: false, draggable: false, 
                    project: elements,
                    route: "/elements"
                },
                { 
                    id: "10", 
                    x: 0, y:0, 
                    width: 1, height: 1, 
                    resizable: false, draggable: false, 
                    project: beru,
                    route: "/beru"
                },
                { 
                    id: "9", 
                    x: 0, y: 0, 
                    width: 1, height: 1, 
                    resizable: false, draggable: false, 
                    project: boltstorm,
                    route: "/bolt_storm"
                },
                { 
                    id: "8", 
                    x: 0, y: 0, 
                    width: 1, height: 1, 
                    resizable: false, draggable: false, 
                    project: automatician,
                    route: "/automatician"
                },
                { 
                    id: "7", 
                    x: 0, y: 0, 
                    width: 1, height: 1, 
                    resizable: false, draggable: false, 
                    project: samsung,
                    route: "/samsung"
                },
                { 
                    id: "6", 
                    x: 0, y: 0,
                    width: 1, height: 1, 
                    resizable: false, draggable: false, 
                    project: runner,
                    route: "/runner"
                },
                { 
                    id: "5", 
                    x: 0, y: 0, 
                    width: 1, height: 1, 
                    resizable: false, draggable: false, 
                    project: free_fowling,
                    route: "/free_fowling"
                },
                {
                    id: "4", 
                    x: 0, y: 0, 
                    width: 1, height: 1, 
                    resizable: false, draggable: false, 
                    project: one_eyed_pirate,
                    route: "/one_eyed_pirate"
                },
                { 
                    id: "3", 
                    x: 0, y: 0, 
                    width: 1, height: 1, 
                    resizable: false, draggable: false, 
                    project: cross_river,
                    route: "/cross_the_river"
                },
                { 
                    id: "2", 
                    x:0, y:0, 
                    width: 1, height: 1, 
                    resizable: false, draggable: false, 
                    project: recovry,
                    route: "/recovry"
                },
                { 
                    id: "1", 
                    x:0, y:0, 
                    width: 1, height: 1, 
                    resizable: false, draggable: false, 
                    project: layhgobuilder,
                    route: "/layhgobuilder",
                },
                {
                    id: "0",
                    x: 0, y: 0,
                    width: 1, height: 1,
                    resizable: false, draggable: false,
                    project: histera,
                    route: "/histera",
                }
            ]
        }
    ]
}