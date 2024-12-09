<template>
{{ $t(source) }} 
</template>

<script>
export default {
    props: {
        source: {
            type: String,
            required: true
        }
    }
}
</script>