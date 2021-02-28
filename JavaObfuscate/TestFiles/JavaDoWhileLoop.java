package sg.test;

public class Test {

    public static int test() {
        int f = 1;
        int i = 0;
        
        do {
            f = f + i;
            i ++;
        }
        while (i < 10);

        return f;
    }
}
