// import { registerOtel } from "@medusajs/medusa"
// // If using an exporter other than Zipkin, require it here.
// import { ZipkinExporter } from "@opentelemetry/exporter-zipkin"
//
// // If using an exporter other than Zipkin, initialize it here.
// const exporter = new ZipkinExporter({
//   serviceName: 'my-medusa-project',
// })
//
// export function register() {
//   registerOtel({
//     serviceName: 'medusajs',
//     // pass exporter
//     exporter,
//     instrument: {
//       http: true,
//       workflows: true,
//       query: true
//     },
//   })
// }