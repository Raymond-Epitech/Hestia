<template>
    <div>
        <div class="month-picker">
            <button class="month-picker-button" @click="prevMonth">‹</button>
            <div class="month">
                {{ dayjs().year(currentYear).month(currentMonth).format("MMMM YYYY") }}
            </div>
            <button class="month-picker-button" @click="nextMonth">›</button>
        </div>

        <div class="weekdays-container">
            <div v-for="d in weekdays" :key="d" class="weekdays">
                {{ $t(`weekdays.${d}`) }}
            </div>
        </div>

        <div class="days-container">
            <div v-for="day in days" :key="day.format('YYYY-MM-DD')" class="days" :class="{
                'not-current-month': day.month() !== currentMonth,
                'bg-blue-100': day.isSame(today, 'day')
            }">
                <div>{{ day.date() }}</div>
                <div v-if="choresByDay[day.format('YYYY-MM-DD')]" class="dot-container">
                    <div v-for="chore in choresByDay[day.format('YYYY-MM-DD')]" :key="chore.id">
                        <CalendarDot :id="chore.id" :title="chore.title" :description="chore.description"
                            :color="getColor(chore.dueDate)" :dueDate="chore.dueDate" :isDone="chore.isDone"
                            :enrolledUsers="chore.enrolledUsers" @proceed="emitProceed()" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>


<script setup lang="ts">
import dayjs from "dayjs";
import 'dayjs/locale/de';
import 'dayjs/locale/es';
import 'dayjs/locale/ja';
import 'dayjs/locale/zh';
import 'dayjs/locale/en';
import 'dayjs/locale/fr';
import weekday from "dayjs/plugin/weekday";
import isoWeek from "dayjs/plugin/isoWeek";
import { useUserStore } from '~/store/user';
import type { Chore } from '../composables/service/type';
import { useI18n } from 'vue-i18n'

const props = defineProps({
    task_list: {
        type: Object as PropType<Chore[]>,
        required: true,
    }
})

dayjs.extend(weekday);
dayjs.extend(isoWeek);

const today = dayjs();
const currentMonth = ref(today.month());
const currentYear = ref(today.year());
const userStore = useUserStore();
const { $bridge } = useNuxtApp()
const api = $bridge;
api.setjwt(useCookie('token').value ?? '');
const weekdays = ['mon', 'tue', 'wed', 'thu', 'fri', 'sat', 'sun'];
const { locale } = useI18n()

const emit = defineEmits(['proceed', 'get'])

function emitProceed() {
    emit('proceed')
}

function getMonthDays(year: number, month: number) {
    const startOfMonth = dayjs().year(year).month(month).startOf("month");
    const endOfMonth = dayjs().year(year).month(month).endOf("month");

    const startGrid = startOfMonth.startOf("week");
    const endGrid = endOfMonth.endOf("week");

    const days: dayjs.Dayjs[] = [];
    let current = startGrid;

    while (current.isBefore(endGrid) || current.isSame(endGrid)) {
        days.push(current);
        current = current.add(1, "day");
    }

    return days;
}

const days = computed(() =>
    getMonthDays(currentYear.value, currentMonth.value)
);

const choresByDay = computed(() => {
    const map: Record<string, any[]> = {};
    props.task_list.forEach(chore => {
        const dateKey = dayjs(chore.dueDate).format("YYYY-MM-DD");
        if (!map[dateKey]) {
            map[dateKey] = [];
        }
        map[dateKey].push(chore);
    });
    return map;
});

function prevMonth() {
    if (currentMonth.value === 0) {
        currentMonth.value = 11;
        currentYear.value--;
    } else {
        currentMonth.value--;
    }
}

function nextMonth() {
    if (currentMonth.value === 11) {
        currentMonth.value = 0;
        currentYear.value++;
    } else {
        currentMonth.value++;
    }
}

function getColor(dueDate: any) {
    const date = new Date(dueDate);
    const today = new Date();
    const daysDifference = Math.ceil((date - today) / (1000 * 3600 * 24));
    if (daysDifference < 1) {
        return "red"
    } else if (daysDifference < 3) {
        return "orange"
    } else if (daysDifference >= 3) {
        return "green"
    } else {
        return "red"
    }
}

watch(locale, (newLocale) => {
    dayjs.locale(newLocale)
}, { immediate: true })
</script>

<style scoped>
.month-picker {
    display: flex;
    justify-content: space-between;
    font-size: 20px;
    font-weight: 600;
    margin-bottom: 4px;
    color: var(--secondary-page-text);
}

.month-picker-button {
    width: 25px;
    background-color: var(--sent-message);
    color: var(--page-text);
    border-radius: 7px;
}

.weekdays-container {
    display: grid;
    grid-template-columns: repeat(7, 1fr);
    grid-gap: 4px;
    text-align: center;
    justify-content: center;
    align-items: center;
    margin-bottom: 4px;
}

.weekdays {
    height: 44px;
    display: flex;
    align-items: center;
    justify-content: center;
    background-color: var(--sent-message);
    color: var(--page-text);
    border-radius: 7px;
    font-weight: 600;
    font-size: 20px;
}

.days-container {
    display: grid;
    grid-template-columns: repeat(7, 1fr);
    grid-gap: 4px;
}

.days {
    height: 90px;
    background-color: var(--recieved-message);
    border-radius: 7px;
    font-weight: 600;
    font-size: 20px;
    text-align: center;
    padding: 4px 10%;
    overflow: scroll;
    color: var(--page-text);
}

.not-current-month {
    color: var(--basic-grey);
}

.dark .not-current-month {
    color: var(--light-grey);
}

.vegan .not-current-month {
    color: var(--light-grey);
}

.dot-container {
    display: grid;
    grid-template-columns: repeat(3, 33%);
    grid-gap: 2px;
}
</style>