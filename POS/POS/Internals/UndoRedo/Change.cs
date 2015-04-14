// Siarhei Arkhipenka (c) 2006-2007. email: sbs-arhipenko@yandex.ru

using System;

namespace POS.Internals.UndoRedo
{
    internal class Change<TState>
    {
        public TState OldState;
        public TState NewState;
    }
}