import express from 'express';
import { Request, Response } from 'express';
import { interval } from 'rxjs';
import { take, map, finalize } from 'rxjs/operators';

const app = express();
const PORT = process.env.PORT || 3000;

app.get('/stream', (req: Request, res: Response) => {
    res.setHeader('Content-Type', 'text/event-stream');
    res.setHeader('Cache-Control', 'no-cache');
    res.setHeader('Connection', 'keep-alive');

    const stream$ = interval(1000).pipe(
        take(10),
        map(() => new Date().toISOString()),
        finalize(() => res.end())
    );

    stream$.subscribe({
        next: (data) => {
            res.write(`data: ${data}\n\n`);
        },
        error: (err) => {
            console.error('Stream error:', err);
            res.end();
        }
    })
});

app.listen(PORT, () => {
    console.log(`Server is running on http://localhost:${PORT}`);
});