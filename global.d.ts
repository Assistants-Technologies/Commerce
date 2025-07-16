declare global {
    interface String {
        segment(pos: number): string | undefined
    }
}