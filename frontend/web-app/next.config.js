/** @type {import('next').NextConfig} */
const nextConfig = {
    // experimental:{
    //     serverActions:true
    // },
    images: {
        domains:[
            'cdn.pixabay.com'
        ]
    },
    logging: {
        fetches: {
            fullUrl: true,
        },
    }
}

module.exports = nextConfig
